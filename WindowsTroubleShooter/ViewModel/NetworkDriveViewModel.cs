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
    public class NetworkDriveViewModel :INotifyPropertyChanged
    {
        private readonly NetworkDriveModel _networkDriveModel;

        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public ICommand MapDriveCommandAsync { get; }

        // Constructor
        public NetworkDriveViewModel()
        {
            _networkDriveModel = new NetworkDriveModel(); // Create instance of the model
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
