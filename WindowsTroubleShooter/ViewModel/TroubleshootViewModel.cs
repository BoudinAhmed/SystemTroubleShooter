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
using System.Windows.Input;

namespace WindowsTroubleShooter.ViewModel
{
    public class TroubleshootViewModel : INotifyPropertyChanged
    {
     
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
                        IssuesToTroubleshoot.Add(new NetworkDriveViewModel());
                        break;
                    case "SearchBar":
                        //_issuesToTroubleshoot.Add(new SearchBarViewModel());
                        break;
                        // Add cases for other issues...
                }
            }

            RunDiagnosticsCommand = new RelayCommand(async () => await RunDiagnosticsAsync());
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public async Task RunDiagnosticsAsync()
        {
            foreach(var issue in IssuesToTroubleshoot)
            {

                await issue.RunDiagnosticsAsync();
            }
        }
    }
}
