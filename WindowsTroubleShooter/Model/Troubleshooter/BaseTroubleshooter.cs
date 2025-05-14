using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WindowsTroubleShooter.Model.Troubleshooter
{
    public abstract class BaseTroubleshooter : INotifyPropertyChanged
    {
        // Backing field for StatusMessage
        private string _statusMessage;

        // Public property with INotifyPropertyChanged implementation
        public string StatusMessage
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

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        // Public properties
        public string IssueType { get; set; }
        public string Detail { get; set; }
        public List<string> TaskList { get; set; } = new List<string>(); // Initialize TaskList
        public DateTime TimeStamp { get; set; }
        public string ResolutionMessage { get; set; }
        public bool IsFixed { get; set; }


        public struct TroubleshootingStep
        {
            public string Description;       
            public string ScriptPath;        
            public string ScriptArguments;   
            public bool IsCritical;          
            
        }



        // Executes a PowerShell script and captures its output and errors.
        protected async Task<(string StandardOutput, string StandardError, int ExitCode)> ExecutePowerShellScriptAsync(string relativeScriptPath, string arguments = "")
        {
            // Convert relative path to absolute path
            string executionDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string scriptPath = Path.GetFullPath(Path.Combine(executionDirectory, relativeScriptPath));
           

            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException($"PowerShell script not found at: {scriptPath}");
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-ExecutionPolicy Bypass -NoProfile -NonInteractive -File \"{scriptPath}\" {arguments}",
                RedirectStandardOutput = true,  
                RedirectStandardError = true,  
                UseShellExecute = false,        
                CreateNoWindow = true
            };

            string standardOutput = string.Empty;
            string standardError = string.Empty;
            int exitCode = -1;

            try
            {
                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    standardOutput = await process.StandardOutput.ReadToEndAsync();
                    standardError = await process.StandardError.ReadToEndAsync();
                    await Task.Run(() => process.WaitForExit());
                    exitCode = process.ExitCode;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error executing PowerShell script: {ex.Message}");
                standardError += $"\nException: {ex.Message}";
                exitCode = -2;
            }

            return (standardOutput, standardError, exitCode);
        }


        public abstract Task<string> RunDiagnosticsAsync();


        protected virtual async Task<(bool IsSuccess, string Message)> ExecuteTroubleshootingStepAsync(TroubleshootingStep step)
        {
            StatusMessage = $"{step.Description}";
            Debug.WriteLine($"Executing Step: {step.Description}");
            await Task.Delay(500);

            
                // Execute the PowerShell script for the step
                var (output, error, exitCode) = await ExecutePowerShellScriptAsync(step.ScriptPath, step.ScriptArguments);

                Debug.WriteLine($"  Script: {step.ScriptPath}");
                Debug.WriteLine($"  Arguments: {step.ScriptArguments}");
                Debug.WriteLine($"  Exit Code: {exitCode}");
                Debug.WriteLine($"  Output: {(string.IsNullOrWhiteSpace(output) ? "(empty)" : output)}");
                Debug.WriteLine($"  Error: {(string.IsNullOrWhiteSpace(error) ? "(empty)" : error)}");

                if (exitCode != 0)
                {
                    StatusMessage = $"{step.Description} Failed!";
                    Debug.WriteLine($"  Step Failed: {step.Description}");
                    // Return error message and indicate failure
                    return (false, $"Error during '{step.Description}': Exit Code {exitCode}. Error: {error}. Output: {output}");
                }
                else
                {
                    StatusMessage = $"{step.Description} Completed Successfully.";
                    Debug.WriteLine($"  Step Succeeded: {step.Description}");
                    // Return output and indicate success
                    return (true, $"'{step.Description}' Successful. Output: {output}");
                }

            
            
            }
    }
}