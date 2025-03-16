
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
using WindowsTroubleShooter.Model;

namespace WindowsTroubleShooter.ViewModel
{
    public class TroubleshootViewModel : INotifyPropertyChanged, IIssueViewModel
    {

        public INavigateService _navigationService { get; set; }

        public ICommand NavigateToTroubleshootingCommand { get; }

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

        // List of issues to troubleshoot
        public List<IIssueViewModel> IssuesToTroubleshoot { get; } = new List<IIssueViewModel>();

        // Contructor to initialize the View
        public TroubleshootViewModel()
        {
        }

        // Constructor that initializes the ViewModel based on selected issues
        public TroubleshootViewModel(ObservableCollection<string> selectedIssues)
        {
            // Dynamically initialize ViewModels based on selected issues
            foreach (var issue in selectedIssues)
            {
                switch (issue)
                {
                    case "NetworkDrive":
                        var networkDriveViewModel = new NetworkDriveViewModel();
                        networkDriveViewModel.PropertyChanged += IssueStatusChanged;
                        IssuesToTroubleshoot.Add(networkDriveViewModel);
                        break;

                    case "SearchBar":
                        //_issuesToTroubleshoot.Add(new SearchBarViewModel());
                        break;
                        // To add other cases...
                }
            }
            
            Task.Run(async () => await RunDiagnosticsAsync());


        

        
    }
        public void NavigateToExit()
        {
            // Call NavigateTo on the navigation service to go to the exit view
            _navigationService.NavigateTo<ExitViewModel>();
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
            if (e.PropertyName == nameof(IIssueViewModel.StatusMessage))
            {
                if (sender is IIssueViewModel issue)
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