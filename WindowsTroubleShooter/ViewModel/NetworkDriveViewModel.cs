using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WindowsTroubleShooter.Helpers;
using WindowsTroubleShooter.Model;

namespace WindowsTroubleShooter.ViewModel
{
    public class NetworkDriveViewModel : IIssueViewModel
    {
        private readonly NetworkDriveModel _networkDriveModel;

        public string StatusMessage { get; set; }



        // Constructor
        public NetworkDriveViewModel()
        {
            _networkDriveModel = new NetworkDriveModel(); // Creating instance of the model
        }

        internal async Task MapNetworkDrive(char driveLetter, string networkPath)
        {
            StatusMessage = $"Searching for {driveLetter}: drive...";
            await Task.Delay(600);
            StatusMessage = "Mapping network drive...";
            await Task.Delay(600);

            // Call the Model and get the result
            StatusMessage = _networkDriveModel.MapNetworkDrive(driveLetter, networkPath);
        }

        

        public async Task RunDiagnosticsAsync()
        {
            await MapNetworkDrive('J', @"\\eprod-st-file01");
        }

        Task IIssueViewModel.RunDiagnosticsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
