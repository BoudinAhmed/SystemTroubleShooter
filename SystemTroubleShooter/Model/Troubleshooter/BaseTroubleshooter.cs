using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text; 
using System.Threading.Tasks;
using System.Windows.Threading; 

namespace SystemTroubleShooter.Model.Troubleshooter
{
    public abstract class BaseTroubleshooter : INotifyPropertyChanged
    {
        // Backing field for StatusMessage
        private string? _statusMessage;

        // Public property with INotifyPropertyChanged implementation
        public string? StatusMessage
        {
            get => _statusMessage;
            set
            {
                // Need to be on the UI thread
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    if (_statusMessage != value)
                    {
                        _statusMessage = value;
                        OnPropertyChanged(nameof(StatusMessage));
                    }
                });
            }
        }

        // Property to hold detailed log output that the UI can bind to
        private string? _detailedLog;
        public string? DetailedLog
        {
            get => _detailedLog;
            set
            {
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    if (_detailedLog != value)
                    {
                        _detailedLog = value;
                        OnPropertyChanged(nameof(DetailedLog));
                    }
                });
            }
        }


        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        // Public properties
        public string? IssueType { get; set; }
        public string? Detail { get; set; }
        public List<string> TaskList { get; set; } = new List<string>(); // Initialize TaskList
        public DateTime TimeStamp { get; set; }
        public string? ResolutionMessage { get; set; }
        public bool IsFixed { get; set; }


        public struct TroubleshootingStep
        {
            public string Description;
            public string ScriptPath;
            public string ScriptArguments;
            public bool IsCritical;
        }


        // Executes a PowerShell script and captures its output and errors.
        protected async Task<(string StandardOutput, string StandardError, int ExitCode)> ExecutePowerShellScriptAsync(
            string relativeScriptPath,
            string arguments = "",
            Action<string>? onOutputDataReceived = null, // Action to call for each line of standard output in the ps script
            Action<string>? onErrorDataReceived = null)  // Action to call for each line of standard output in the ps script
        {
            // Convert relative path to absolute path
            string executionDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string scriptPath = Path.GetFullPath(Path.Combine(executionDirectory, relativeScriptPath));

            if (!File.Exists(scriptPath))
            {
                onErrorDataReceived?.Invoke($"PowerShell script not found at: {scriptPath}");
                return ("", $"PowerShell script not found at: {scriptPath}", -1); // Indicate script file not found
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-ExecutionPolicy Bypass -NoProfile -NonInteractive -File \"{scriptPath}\" {arguments}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true, // Set to true to hide the console window
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            // Use StringBuilders to accumulate output for the final return value
            var standardOutputBuilder = new StringBuilder();
            var standardErrorBuilder = new StringBuilder();
            int exitCode = -1;

            try
            {
                using (var process = new Process { StartInfo = startInfo })
                {
                    // Attach event handlers BEFORE calling Start()
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            standardOutputBuilder.AppendLine(e.Data);
                            onOutputDataReceived?.Invoke(e.Data);
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            standardErrorBuilder.AppendLine(e.Data);
                            onErrorDataReceived?.Invoke(e.Data);
                        }
                    };

                    process.Start();

                    // Begin asynchronous reading of the output and error streams
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    // Wait asynchronously for the process to exit
                    await Task.Run(() => process.WaitForExit());

                    // Ensure all output streams have been flushed and events fired before proceeding
                    process.WaitForExit();

                    exitCode = process.ExitCode;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error executing PowerShell script process: {ex.Message}");
                string processError = $"Exception starting or running PowerShell script: {ex.Message}";
                standardErrorBuilder.AppendLine(processError);
                onErrorDataReceived?.Invoke(processError);
                exitCode = -2;
            }

            // Return the accumulated output/error and the final exit code
            return (standardOutputBuilder.ToString(), standardErrorBuilder.ToString(), exitCode);
        }


        public abstract Task<string> RunDiagnosticsAsync();


        // Orchestrates troubleshooting steps and handles real-time output.
        protected virtual async Task<(bool IsSuccess, string Message)> ExecuteTroubleshootingStepAsync(TroubleshootingStep step)
        {
            // Set initial status message for the step
            StatusMessage = $"{step.Description}...";
            

            Debug.WriteLine($"Executing Step: {step.Description}");
            await Task.Delay(100); // Small delay to allow UI update before script


            // --- Define Actions for Real-Time Output/Error ---
            Action<string> handleOutput = (data) =>
            {
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    DetailedLog += data + Environment.NewLine;
                    StatusMessage = data;
                    
                });
            };

            Action<string> handleError = (data) =>
            {
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    DetailedLog += "ERROR: " + data + Environment.NewLine;

                });
            };


            // Execute the PowerShell script for the step in real tim
            var (finalOutput, finalError, exitCode) = await ExecutePowerShellScriptAsync(
                step.ScriptPath,
                step.ScriptArguments,
                onOutputDataReceived: handleOutput, // Pass the output 
                onErrorDataReceived: handleError   // Pass the error 
            );

            // --- After the script finishes ---

            Debug.WriteLine($"  Script: {step.ScriptPath}");
            Debug.WriteLine($"  Arguments: {step.ScriptArguments}");
            Debug.WriteLine($"  Exit Code: {exitCode}");
            Debug.WriteLine($"  Output: {(string.IsNullOrWhiteSpace(finalOutput) ? "(empty)" : finalOutput)}");
            Debug.WriteLine($"  Error: {(string.IsNullOrWhiteSpace(finalError) ? "(empty)" : finalError)}");


            bool isSuccess = (exitCode == 0);
            string resultMessage;

            if (!isSuccess)
            {
                // Update StatusMessage to indicate failure after script exit
                StatusMessage = $"{step.Description} Failed (Code: {exitCode}).";
                Debug.WriteLine($"  Step Failed: {step.Description}");
                // Construct the result message including captured output or error
                resultMessage = $"Error during '{step.Description}'. Exit Code: {exitCode}.\nError Output:\n{finalError}\nStandard Output:\n{finalOutput}";

            }
            else
            {
                // Update StatusMessage to indicate success after script exit
                StatusMessage = $"{step.Description} Completed Successfully.";
                Debug.WriteLine($"  Step Succeeded: {step.Description}");

                resultMessage = $"'{step.Description}' Successful.\n\nDetails:\n{finalOutput}"; // Include full output details
            }

            // Return the overall success orfailure 
            return (isSuccess, resultMessage);
        }
    }
}