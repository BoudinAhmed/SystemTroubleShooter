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
        public class IssueItemViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;

            private bool areButtonsVisible;
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

            private bool isTextVisible = true;
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

            public Border AssociatedBorder { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string ImageSource { get; set; }
            public bool IsItemSelected { get; set; }

            public ICommand ItemClickedCommand { get; }
            public ICommand ItemCancelClickedCommand { get; }
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
                if (AssociatedBorder == null) return;

                var fadeOutStoryboard = AssociatedBorder.Resources["FadeOut"] as Storyboard;
                if (fadeOutStoryboard == null) return;

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
                if (AssociatedBorder == null) return;

                var fadeInStoryboard = AssociatedBorder.Resources["FadeIn"] as Storyboard;
                var resetStoryboard = AssociatedBorder.Resources["ResetAnimation"] as Storyboard;

                if (fadeInStoryboard == null || resetStoryboard == null) return;

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
                // Initialize troubleshoot
                throw new NotImplementedException();
            }

            public void ShowButtons()
            {
                IsItemSelected = true;
                IsTextVisible = false;
                AreButtonsVisible = true;
            }

            private void StartButtonAnimation(object sender)
            {
                Reset();
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

            public void Reset()
            {
                if (IsTextVisible || !AreButtonsVisible) return;

                if (AssociatedBorder == null) return;

                var fadeInStoryboard = AssociatedBorder.Resources["FadeIn"] as Storyboard;
                var resetStoryboard = AssociatedBorder.Resources["ResetAnimation"] as Storyboard;

                if (fadeInStoryboard == null) return;

                resetStoryboard.Completed += (sender, e) => fadeInStoryboard.Begin(AssociatedBorder);
                resetStoryboard.Begin(AssociatedBorder);

                IsItemSelected = false;
                IsTextVisible = true;
                AreButtonsVisible = false;
            }

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        
       
    }