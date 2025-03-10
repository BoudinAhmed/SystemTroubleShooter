using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsTroubleShooter.ViewModel
{
    class TroubleshootViewModel
    {
        private ObservableCollection<string> _selectedIssues { get; set; }
        private NetworkDriveViewModel _networkDriveViewModel { get; set; }
        public TroubleshootViewModel(ObservableCollection<string> SelectedIssues)
        {
            this._selectedIssues = SelectedIssues;
            _networkDriveViewModel = new NetworkDriveViewModel();
        }

        private async Task RunDiagnosticsAsync()
        {
            foreach(var issue in _selectedIssues)
            {
                if(issue == "NetworkDrive")
                {
                    await _networkDriveViewModel.MapNetworkDrive('Z', @"\\network\path");
                }
                else if(issue == "SearchBar")
                {

                }
            }
        }
    }
}
