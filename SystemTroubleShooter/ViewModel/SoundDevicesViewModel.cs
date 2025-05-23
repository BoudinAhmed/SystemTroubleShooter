using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SystemTroubleShooter.Helpers.Commands;

namespace SystemTroubleShooter.ViewModel
{
    public class SoundDevicesViewModel : ViewModelBase
    {

        private ObservableCollection<string> _devices;

        public ObservableCollection<string> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged(nameof(Devices));
            }
        }

        public ICommand LoadOutputDevicesCommand { get; private set; }
        public ICommand LoadInputDevicesCommand { get; private set; }

        public SoundDevicesViewModel()
        {
            Devices = new ObservableCollection<string>();
            LoadOutputDevicesCommand = new RelayCommand(LoadOutputDevices);
            LoadInputDevicesCommand = new RelayCommand(LoadInputDevices);
        }

        private void LoadOutputDevices(object obj)
        {
            // Simulate fetching output devices
            List<string> outputDevices = new List<string>
            {
                "Speakers (Realtek High Definition Audio)",
                "Headphones (Logitech G Pro X Gaming Headset)",
                "Monitor Audio (NVIDIA High Definition Audio)"
            };
            Devices = new ObservableCollection<string>(outputDevices);
        }

        private void LoadInputDevices(object obj)
        {
            // Simulate fetching input devices
            List<string> inputDevices = new List<string>
            {
                "Microphone (Logitech G Pro X Gaming Headset)",
                "Line In (Realtek High Definition Audio)",
                "Webcam Microphone (Logitech C920)"
            };
            Devices = new ObservableCollection<string>(inputDevices);
        }
    }
}
