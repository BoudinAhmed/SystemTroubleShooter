using Caliburn.Micro;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WindowsTroubleShooter.ViewModel
{
    public class TroubleshootViewModel : INotifyPropertyChanged, IIssueViewModel
    {
        public TroubleshootViewModel()
        {
        }

        public String StatusMessage { get; set; }
        public List<IIssueViewModel> IssuesToTroubleshoot = new List<IIssueViewModel>();
        
        public ICommand RunDiagnosticsCommand { get; }

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
                        IssuesToTroubleshoot.Add(new NetworkDriveViewModel());

                        
                        break;
                    case "SearchBar":
                        //_issuesToTroubleshoot.Add(new SearchBarViewModel());
                        break;
                        // Add cases for other issues...
                }
            }

            Task.Run(async ()=> await RunDiagnosticsAsync());
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void IssueStatusChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IIssueViewModel.StatusMessage))
            {
                var issue = sender as IIssueViewModel;
                if (issue != null)
                {
                    StatusMessage = issue.StatusMessage; // Update the main StatusMessage property
                }
            }
        }

        public async Task RunDiagnosticsAsync()
        {
            // Run diagnostics in a background task to avoid blocking the UI thread.
            await Task.Run(async () =>
            {
                foreach (var issue in IssuesToTroubleshoot)
                {
                    await issue.RunDiagnosticsAsync();

                    // To update the UI, use the Dispatcher to marshal the updates to the UI thread.
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        StatusMessage = issue.StatusMessage;
                    });

                    await Task.Delay(500); // Add delay if needed
                }
            });
        }
    }
}
