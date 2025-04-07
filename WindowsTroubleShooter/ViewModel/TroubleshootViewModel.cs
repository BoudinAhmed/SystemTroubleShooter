
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WindowsTroubleShooter.Helpers;
using WindowsTroubleShooter.Interfaces;
using WindowsTroubleShooter.Model;

namespace WindowsTroubleShooter.ViewModel
{
    public class TroubleshootViewModel : INotifyPropertyChanged, IIssue
    {
        

        private string _statusMessage;
        // Property to bind the StatusMessage with the UI
        public string StatusMessage
        {
            get => _statusMessage;
            private set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }
        public object CurrentView { get; set; } // CurrentView property

        // List of issues to troubleshoot
        public List<IIssue> IssuesToTroubleshoot { get; } = new List<IIssue>();

        // Contructor to initialize the View
        public TroubleshootViewModel()
        {
        }

        // Constructor that initializes the ViewModel based on selected issues
        public TroubleshootViewModel(BaseTroubleshooter selectedIssue)
        {
            StatusMessage = selectedIssue.StatusMessage;
            
            Task.Run(async () => await selectedIssue.RunDiagnosticsAsync());


        

        
    }
        public void NavigateToExit()
        {
            // Todo: Change to resolution view upon completing troubleshooting (with final message)
            
        }


        // Event to notify changes in the properties
        public event PropertyChangedEventHandler PropertyChanged;

        // Trigger PropertyChanged notification
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Event handler to update the StatusMessage when an issue's StatusMessage changes
        private void IssueStatusChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BaseTroubleshooter.StatusMessage))
            {
                if (sender is BaseTroubleshooter issue)
                {
                    StatusMessage = issue.StatusMessage;
                }
            }
        }

        // Run diagnostics on all issues asynchronously
        public async Task RunDiagnosticsAsync()
        {
            foreach (var issue in IssuesToTroubleshoot)
            {
                await issue.RunDiagnosticsAsync();

                // Update the StatusMessage based on the current issue
                StatusMessage = issue.StatusMessage;

                // Adding a small delay to avoid blocking
                await Task.Delay(500);
            }
            NavigateToExit();
        }
    }
}