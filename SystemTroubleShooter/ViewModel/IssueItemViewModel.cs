using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using SystemTroubleShooter.Helpers;
using SystemTroubleShooter.Helpers.Commands;
using SystemTroubleShooter.Interfaces;
using SystemTroubleShooter.Model.Troubleshooter;

namespace SystemTroubleShooter.ViewModel
{
    // Represents an issue item in the UI elements.
    public class IssueItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private BaseTroubleshooter? _currentIssue;
        private bool _areButtonsVisible;
        private bool _isTextVisible = true;
        private Border? _associatedBorder;
        private bool _isTroubleshooting;
        private string? _troubleshootingStatus;

        // To display audiodevices view
        private readonly IAudioDeviceDisplayService _audioDeviceDisplayService;


        // Gets or sets a value indicating whether the buttons are visible.
        public bool AreButtonsVisible
        {
            get => _areButtonsVisible;
            set
            {
                if (_areButtonsVisible != value)
                {
                    _areButtonsVisible = value;
                    OnPropertyChanged(nameof(AreButtonsVisible));
                }
            }
        }

        // Gets or sets a value indicating whether the text is visible.
        public bool IsTextVisible
        {
            get => _isTextVisible;
            set
            {
                if (_isTextVisible != value)
                {
                    _isTextVisible = value;
                    OnPropertyChanged(nameof(IsTextVisible));
                }
            }
        }

        // Gets or sets a value indicating whether the troubleshoot items is visible.
        public bool IsTroubleshooting
        {
            get => _isTroubleshooting;
            set
            {
                if (_isTroubleshooting != value)
                {
                    _isTroubleshooting = value;
                    OnPropertyChanged(nameof(IsTroubleshooting));
                }
            }
        }

        // Gets or sets a value for troubleshooting status message
        public string? TroubleshootingStatus
        {
            get => _troubleshootingStatus;
            set
            {
                if (_troubleshootingStatus != value)
                {
                    _troubleshootingStatus = value;
                    OnPropertyChanged(nameof(TroubleshootingStatus));
                }
            }
        }
        // Gets or sets the associated border element.
        public Border? AssociatedBorder
        {
            get => _associatedBorder;
            set => _associatedBorder = value;
        }

        // Gets or sets properties for all elements of an item.
        public string Title { get; set; } = "new Issue Title";
        public string Description { get; set; } = "new Issue Description";
        public string ImageSource { get; set; } = "xE9CE;";
        public BaseTroubleshooter? IssueType { get; set; }
        public bool IsItemSelected { get; set; }

        // Handle the navigation to the next view.
        private NavigationService _navigationService { get; set; }

        // Gets the command executed when the item is clicked.
        public ICommand ItemClickedCommand { get; }

        // Gets the command executed when the item's cancel button is clicked.
        public ICommand ItemCancelClickedCommand { get; }

        // Gets the command executed when the item's troubleshoot button is clicked.
        public ICommand ItemTroubleshootClickedCommand { get; }

        public IssueItemViewModel()
        {
            _navigationService = new NavigationService();
            _audioDeviceDisplayService = new AudioDeviceDisplayService(); // Assign the injected service

            ItemClickedCommand = new RelayCommand(OnItemClicked);
            ItemCancelClickedCommand = new RelayCommand(OnItemCancelClicked);
            ItemTroubleshootClickedCommand = new RelayCommand(OnItemTroubleshootClicked);
        }
        

        private void OnItemClicked(object? obj)
        {
            //Not to interupt the troubleshooting process
            if (IsTroubleshooting) {
                return;
            }

            //The animation to be played when the item is clicked.
            if(obj is not null) 
            AssociatedBorder = (Border)obj ;

            if (AssociatedBorder == null) return;

            //Title and Description fade out first
            var fadeOutStoryboard = AssociatedBorder.Resources["FadeOut"] as Storyboard;
            if (fadeOutStoryboard == null) return;

            fadeOutStoryboard.Completed += (sender, e) => StartButtonAnimation(AssociatedBorder);
            fadeOutStoryboard.Begin(AssociatedBorder);

            if (Application.Current.MainWindow?.DataContext is StartViewModel startViewModel)
            {
                startViewModel.ListenToNextClicked(this, AssociatedBorder);
            }
        }

        private void OnItemCancelClicked(object? obj)
        {
            if (obj is not null)
                AssociatedBorder = (Border)obj;

            if (AssociatedBorder == null) return;

            var fadeInStoryboard = AssociatedBorder.Resources["FadeIn"] as Storyboard;
            var resetStoryboard = AssociatedBorder.Resources["ResetAnimation"] as Storyboard;

            if (fadeInStoryboard == null || resetStoryboard == null) return;

            resetStoryboard.Completed += (sender, e) => fadeInStoryboard.Begin(AssociatedBorder);
            resetStoryboard.Begin(AssociatedBorder);

            IsItemSelected = false;
            IsTextVisible = true;
            AreButtonsVisible = false;
            IsTroubleshooting = false;

            if (Application.Current.MainWindow?.DataContext is StartViewModel startViewModel)
            {
                startViewModel.ListenToNextClicked(this, AssociatedBorder);
            }
        }

        private void IssueStatusChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Check if the property that changed is StatusMessage.
            if (e.PropertyName == nameof(BaseTroubleshooter.StatusMessage))
            {
                if(_currentIssue != null && _currentIssue.StatusMessage != null)
                TroubleshootingStatus = _currentIssue.StatusMessage;
            }
        }

        private async void OnItemTroubleshootClicked(object? obj)
        {
            
            AreButtonsVisible = false; //Buttons (Cancel/Trouble) visibility on issueItem
            IsTroubleshooting = true;
            TroubleshootingStatus = "Starting...";
            _currentIssue = this.IssueType;

            if (_currentIssue == null) 
            {
                TroubleshootingStatus = "Coming soon - Under Development";
                await Task.Delay(5000);
                ResetIssueItemAnimation(obj);
                return; 
            
            }

            _currentIssue.PropertyChanged += IssueStatusChanged;


            if (_currentIssue is SoundTroubleshooter soundTroubleshooter)
            {

                (List<string> InputDevices, List<string> OutputDevices) = await soundTroubleshooter.GetAllAudioDevicesAsync();

                TroubleshootingStatus = "Please select a device...";
                string? userSelectedDevice = await _audioDeviceDisplayService.SelectAudioDeviceAsync(OutputDevices, InputDevices);

                soundTroubleshooter.SelectedDevice = userSelectedDevice;

            }
            
                Task diagnosticsTask = Task.Run(async () => await _currentIssue.RunDiagnosticsAsync());
            

            await diagnosticsTask;

            if (diagnosticsTask.IsCompleted)
            {
                
                if (_currentIssue.IsFixed)
                {
                    TroubleshootingStatus = "Success";
                }
                else
                {
                    TroubleshootingStatus = "Failed";
                }
                await Task.Delay(2000);
                 IsTroubleshooting = false;

                // The animation to reset to inital state
                ResetIssueItemAnimation(obj);
            }
            
        }

        public void ResetIssueItemAnimation(object? obj)
        {
            if (obj is not null)
                AssociatedBorder = (Border)obj;

            if (AssociatedBorder == null) return;

            var fadeInStoryboard = AssociatedBorder.Resources["FadeIn"] as Storyboard;
            var resetStoryboard = AssociatedBorder.Resources["ResetAnimation"] as Storyboard;

            if (fadeInStoryboard == null || resetStoryboard == null) return;

            resetStoryboard.Completed += (sender, e) => fadeInStoryboard.Begin(AssociatedBorder);
            resetStoryboard.Begin(AssociatedBorder);

            IsItemSelected = false;
            IsTextVisible = true;
            AreButtonsVisible = false;
            IsTroubleshooting = false;

            if (Application.Current.MainWindow?.DataContext is StartViewModel startViewModel)
            {
                startViewModel.ListenToNextClicked(this, AssociatedBorder);
            }
        }

        // Shows the buttons associated with the issue item.
        public void ShowButtons()
        {
            IsItemSelected = true;
            IsTextVisible = false;
            AreButtonsVisible = true;
        }

        private void StartButtonAnimation(object sender)
        {
            ResetIssueItemAnimation(sender);
            if (!(sender is Border border)) return;

            var buttonAnimationStoryboard = border.Resources["ButtonAnimation"] as Storyboard;
            if (buttonAnimationStoryboard == null)
            {
                Debug.WriteLine("Storyboard not found");
                return;
            }

            ShowButtons();
            buttonAnimationStoryboard.Begin(border);
        }

        // Resets the issue item to its initial state (title and description shown only).
        public void ResetIssueItem()
        {
            if (IsTextVisible || !AreButtonsVisible) return;
            if (AssociatedBorder == null) return;

            var fadeInStoryboard = AssociatedBorder.Resources["FadeIn"] as Storyboard;
            var resetStoryboard = AssociatedBorder.Resources["ResetAnimation"] as Storyboard ?? throw new ArgumentNullException(nameof(Storyboard), "resetStoryboard cannot be null."); ;

            if (fadeInStoryboard == null) return;

            resetStoryboard.Completed += (sender, e) => fadeInStoryboard.Begin(AssociatedBorder);
            resetStoryboard.Begin(AssociatedBorder);

            IsItemSelected = false;
            IsTextVisible = true;
            AreButtonsVisible = false;
        }

        // PropertyChanged event.
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}