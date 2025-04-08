using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WindowsTroubleShooter.Model
{
    public abstract class BaseTroubleshooter : INotifyPropertyChanged
    {
        // Constants for command execution
        private const string CommandExecutable = "net";

        // Backing field for StatusMessage
        private string _statusMessage;

        // Public property with INotifyPropertyChanged implementation
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));
                }
            }
        }

        // Public properties
        public bool IsFixed { get; set; }
        public string IssueType { get; set; }
        public string Detail { get; set; }
        public List<string> TaskList { get; set; }
        public DateTime TimeStamp { get; set; }
        public string ResolutionMessage { get; set; }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            System.Diagnostics.Debug.WriteLine($"BaseTroubleshooter: OnPropertyChanged called for '{propertyName}' on Thread {System.Threading.Thread.CurrentThread.ManagedThreadId}");


            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Method to execute command and return success status
        public bool ExecuteCommand(string command)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.FileName = CommandExecutable;
                    process.StartInfo.Arguments = $" {command}";
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();
                    process.WaitForExit();

                    string output = process.StandardOutput.ReadToEnd();

                    // Will add code for the 'output' here.

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Consider logging the exception for debugging
                Debug.WriteLine($"Error executing command: {ex.Message}");
                return false;
            }
        }

        // Abstract method for asynchronous diagnostics
        public abstract Task<string> RunDiagnosticsAsync();
    }
}