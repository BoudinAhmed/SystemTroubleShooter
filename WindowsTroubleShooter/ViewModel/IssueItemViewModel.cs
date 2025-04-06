using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using WindowsTroubleShooter.Helpers.Commands;

namespace WindowsTroubleShooter.ViewModel
{
    
    // Represents an issue item in the UI elements.
    public class IssueItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool areButtonsVisible;
        private bool isTextVisible = true;
        private Border associatedBorder;

        
        // Gets or sets a value indicating whether the buttons are visible
        public bool AreButtonsVisible
        {
            get => areButtonsVisible;
            set
            {
                if (areButtonsVisible != value)
                {
                    areButtonsVisible = value;
                    OnPropertyChanged(nameof(AreButtonsVisible));
                }
            }
        }

        
        // Gets or sets a value indicating whether the text is visible
        public bool IsTextVisible
        {
            get => isTextVisible;
            set
            {
                if (isTextVisible != value)
                {
                    isTextVisible = value;
                    OnPropertyChanged(nameof(IsTextVisible));
                }
            }
        }

        
        // Gets or sets the associated border element.
        public Border AssociatedBorder
        {
            get => associatedBorder;
            set
            {
                associatedBorder = value;
            }
        }

        
        // Gets or sets for all elements of an item
        public string Title { get; set; }
        public string Description { get; set; }  
        public string ImageSource { get; set; }
        public bool IsItemSelected { get; set; }

        // Gets the command executed when the item's is clicked. 
        public ICommand ItemClickedCommand { get; }

        
       // Gets the command executed when the item's cancel button is clicked. 
        public ICommand ItemCancelClickedCommand { get; }

        // Gets the command executed when the item's troubleshoot button is clicked. 
        public ICommand ItemTroubleshootClickedCommand { get; }

        
        public IssueItemViewModel()
        {
            ItemClickedCommand = new RelayCommand(OnItemClicked);
            ItemCancelClickedCommand = new RelayCommand(OnItemCancelClicked);
            ItemTroubleshootClickedCommand = new RelayCommand(OnItemTroubleshootClicked);
        }

        private void OnItemClicked(object obj)
        {
            AssociatedBorder = obj as Border;
            if (AssociatedBorder == null)
            {
                return;
            }

            var fadeOutStoryboard = AssociatedBorder.Resources["FadeOut"] as Storyboard;
            if (fadeOutStoryboard == null)
            {
                return;
            }

            fadeOutStoryboard.Completed += (sender, e) => StartButtonAnimation(AssociatedBorder);
            fadeOutStoryboard.Begin(AssociatedBorder);

            if (Application.Current.MainWindow?.DataContext is StartViewModel startViewModel)
            {
                startViewModel.ListenToNextClicked(this, AssociatedBorder);
            }
        }

        private void OnItemCancelClicked(object obj)
        {
            AssociatedBorder = obj as Border;
            if (AssociatedBorder == null)
            {
                return;
            }

            var fadeInStoryboard = AssociatedBorder.Resources["FadeIn"] as Storyboard;
            var resetStoryboard = AssociatedBorder.Resources["ResetAnimation"] as Storyboard;

            if (fadeInStoryboard == null || resetStoryboard == null)
            {
                return;
            }

            resetStoryboard.Completed += (sender, e) => fadeInStoryboard.Begin(AssociatedBorder);
            resetStoryboard.Begin(AssociatedBorder);

            IsItemSelected = false;
            IsTextVisible = true;
            AreButtonsVisible = false;

            if (Application.Current.MainWindow?.DataContext is StartViewModel startViewModel)
            {
                startViewModel.ListenToNextClicked(this, AssociatedBorder);
            }
        }

        private void OnItemTroubleshootClicked(object obj)
        {
            // Todo: implement troubleshoot button
            throw new NotImplementedException();
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
            Reset();
            if (!(sender is Border border))
            {
                return;
            }

            var buttonAnimationStoryboard = border.Resources["ButtonAnimation"] as Storyboard;
            if (buttonAnimationStoryboard == null)
            {
                Debug.WriteLine("Storyboard not found");
                return;
            }

            ShowButtons();
            buttonAnimationStoryboard.Begin(border);
        }

        
        // Resets the issue item to its initial state.
        public void Reset()
        {
            if (IsTextVisible || !AreButtonsVisible)
            {
                return;
            }

            if (AssociatedBorder == null)
            {
                return;
            }

            var fadeInStoryboard = AssociatedBorder.Resources["FadeIn"] as Storyboard;
            var resetStoryboard = AssociatedBorder.Resources["ResetAnimation"] as Storyboard;

            if (fadeInStoryboard == null)
            {
                return;
            }

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
