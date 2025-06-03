using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows;
using SystemTroubleShooter.View;
using System.Linq;
using SystemTroubleShooter.Helpers.Commands;
using System.Collections.Generic;
using System; // Required for Storyboard

namespace SystemTroubleShooter.ViewModel
{
    public class SoundDevicesViewModel : INotifyPropertyChanged
    {
        private bool _hasAnimationsPlayed = false;

        private ObservableCollection<string>? _devices;
        public ObservableCollection<string>? Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged();
            }
        }

        private string? _selectedDevice;
        public string? SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged();
            }
        }


        private bool _isContentVisible;
        private readonly List<string>? outputDevices;
        private readonly List<string>? inputDevices;

        public bool IsContentVisible
        {
            get { return _isContentVisible; }
            set
            {
                _isContentVisible = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadOutputDevicesCommand { get; private set; }
        public ICommand LoadInputDevicesCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public Action<bool?>? CloseWindowAction { get; set; }

        public string? ConfirmedDevice { get; private set; } // To store the confirmed device
        private const string NoInputDeviceFoundMessage = "No input device found";
        private const string NoOutputDeviceFoundMessage = "No output device found";

        public SoundDevicesViewModel()
        {
            Devices = new ObservableCollection<string>();
            IsContentVisible = false; // Initially hidden

            LoadOutputDevicesCommand = new RelayCommand(param => LoadDevices("Output"));
            LoadInputDevicesCommand = new RelayCommand(param => LoadDevices("Input"));
            ConfirmCommand = new RelayCommand(ExecuteConfirm, CanExecuteConfirm);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        public SoundDevicesViewModel(List<string> outputDevices, List<string> inputDevices)
        {
            this.outputDevices = outputDevices;
            this.inputDevices = inputDevices;

            Devices = new ObservableCollection<string>();
            IsContentVisible = false; // Initially hidden

            LoadOutputDevicesCommand = new RelayCommand(param => LoadDevices("Output"));
            LoadInputDevicesCommand = new RelayCommand(param => LoadDevices("Input"));
            ConfirmCommand = new RelayCommand(ExecuteConfirm, CanExecuteConfirm);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        private bool CanExecuteConfirm(object? parameter)
        {
            return !string.IsNullOrEmpty(SelectedDevice) &&
                   SelectedDevice != NoInputDeviceFoundMessage &&
                   SelectedDevice != NoOutputDeviceFoundMessage;
        }

        private void ExecuteConfirm(object? parameter)
        {
            ConfirmedDevice = SelectedDevice;
            CloseWindowAction?.Invoke(true); // true for confirmed
        }

        private void ExecuteCancel(object? parameter)
        {
            ConfirmedDevice = null;
            CloseWindowAction?.Invoke(false); // false for cancelled
        }

        private void LoadDevices(string deviceType)
        {
            Devices ??= new ObservableCollection<string>();

            // Clear existing devices
            Devices.Clear();

            // Simulate loading devices
            if (deviceType == "Input")
            {
                if (inputDevices is not null && inputDevices.Count != 0)
                    foreach(var device in inputDevices)
                    Devices?.Add(device);

                else
                    Devices.Add("No input device found");
            }
            else // Input
            {
                if (outputDevices is not null && outputDevices.Count != 0)
                    foreach (var device in outputDevices)
                        Devices?.Add(device);

                else
                    Devices.Add("No output device found");
            }

            if (!_hasAnimationsPlayed)
                TriggerAnimations();


            IsContentVisible = true;
        }

        private void TriggerAnimations()
        {
            if (_hasAnimationsPlayed == true)
                return;

            
            
            var window = Application.Current.Windows.OfType<SoundDevicesView>().FirstOrDefault();
            if (window != null)
            {
                // Play Fade Out Text
                var fadeOutTextStoryboard = (Storyboard)window.FindResource("FadeOutText");
                fadeOutTextStoryboard.Begin(window); // Pass the window as scope

                // Play Buttons Move Up
                var buttonsMoveUpStoryboard = (Storyboard)window.FindResource("ButtonsMoveUp");
                buttonsMoveUpStoryboard.Begin(window);

                
                var FadeInListViewStoryboard = (Storyboard)window.FindResource("FadeInListView");
                FadeInListViewStoryboard.Begin(window);

                var fadeInConfirmCancelPanelStoryboard = (Storyboard)window.FindResource("FadeInConfirmCancelPanel");
                fadeInConfirmCancelPanelStoryboard.Begin(window);

                _hasAnimationsPlayed = true;
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler ?PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    
}