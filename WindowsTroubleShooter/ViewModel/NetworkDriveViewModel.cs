using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WindowsTroubleShooter.Helpers;
using WindowsTroubleShooter.Model;

namespace WindowsTroubleShooter.ViewModel
{
    public class NetworkDriveViewModel : IIssueViewModel, INotifyPropertyChanged
    {
        private readonly NetworkDriveModel _networkDriveModel;
        private string _statusMessage;

        // Initialize the model
        public NetworkDriveViewModel()
        {
            _networkDriveModel = new NetworkDriveModel();
        }

        // StatusMessage with change notification
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

        // Event to notify when a property changes
        public event PropertyChangedEventHandler PropertyChanged;

        // Trigger PropertyChanged notification
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // map the network drive async
        internal async Task MapNetworkDrive(char driveLetter, string networkPath)
        {
            StatusMessage = $"Searching for {driveLetter}: drive...";
            await Task.Delay(600);

            StatusMessage = "Mapping network drive...";
            await Task.Delay(600);

            var result = _networkDriveModel.MapNetworkDrive(driveLetter, networkPath);
            StatusMessage = result;
        }


        // Run diagnostics asynchronously
        public async Task RunDiagnosticsAsync()
        {
            await MapNetworkDrive('J', @"\\eprod-st-file01");
        }
    }
}