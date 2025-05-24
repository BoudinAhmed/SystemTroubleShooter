using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows;
using SystemTroubleShooter.View;
using System.Linq;
using SystemTroubleShooter.Helpers.Commands; // Required for Storyboard

namespace SystemTroubleShooter.ViewModel
{
    public class SoundDevicesViewModel : INotifyPropertyChanged
    {
        private bool _hasAnimationsPlayed = false;

        private ObservableCollection<string> _devices;
        public ObservableCollection<string> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged();
            }
        }

        private bool _isContentVisible;
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

        public SoundDevicesViewModel()
        {
            Devices = new ObservableCollection<string>();
            IsContentVisible = false; // Initially hidden

            LoadOutputDevicesCommand = new RelayCommand(param => LoadDevices("Output"));
            LoadInputDevicesCommand = new RelayCommand(param => LoadDevices("Input"));
        }

        private void LoadDevices(string deviceType)
        {
            // Clear existing devices
            Devices.Clear();

            // Simulate loading devices
            if (deviceType == "Output")
            {
                Devices.Add("Realtek High Definition Audio");
                Devices.Add("NVIDIA Output (NVIDIA High Definition Audio)");
                Devices.Add("USB Speakers");
            }
            else // Input
            {
                Devices.Add("Microphone (Realtek High Definition Audio)");
                Devices.Add("Webcam Microphone");
            }

            // Trigger animations
            TriggerAnimations();
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

                _hasAnimationsPlayed = true;
                IsContentVisible = true;
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