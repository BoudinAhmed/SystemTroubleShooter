using System;
using System.Collections.ObjectModel;
using WindowsTroubleShooter.Interfaces;
using System.Windows.Controls;
using WindowsTroubleShooter.Model;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using WindowsTroubleShooter.View;
using System.Windows.Media.Animation;
using System.Windows;

namespace WindowsTroubleShooter.ViewModel
{
    public class StartViewModel : ViewModelBase
    {
        private IssueItemViewModel _lastClickedItem;
        private Border _lastClickedBorder;       
        private UserControl _currentUserControl = new DashboardView();
        public ICommand SwitchToDashboardCommand { get; set; }
        public ICommand SwitchToProblemListCommand { get; set; }


        public UserControl CurrentUserControl
        {
            get { return _currentUserControl; }
            set { SetProperty(ref (_currentUserControl), value); }
        } 

        public StartViewModel() 
        {
            SwitchToDashboardCommand = new RelayCommand(SwitchToDashboard);
            SwitchToProblemListCommand = new RelayCommand(SwitchToProblemList);
        }

        private void SwitchToDashboard()
        {
            if (CurrentUserControl is not DashboardView)
            {
                FadeOut(() =>
            {
                CurrentUserControl = new DashboardView();
                OnPropertyChanged(nameof(CurrentUserControl));
                FadeIn();
            });

            }
        }

        private void SwitchToProblemList()
        {
            if (CurrentUserControl is not ProblemListView)
            {
                FadeOut(() =>
                {
                    CurrentUserControl = new ProblemListView();
                    OnPropertyChanged(nameof(CurrentUserControl));
                    FadeIn();
                });
            }
        }

        //Animation when switch usercontrols
        private void FadeOut(Action onComplete)
        {
            var fadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.1)));
            fadeOut.Completed += (s, e) => onComplete();
            (_currentUserControl as UIElement)?.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        private void FadeIn()
        {
            var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.2)));
            (_currentUserControl as UIElement)?.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        }

        //To cancelled/reset selection if another issueItem is clicked
        public void ListenToNextClicked(IssueItemViewModel clickedItem, Border clickedBorder)
        {
            if (_lastClickedItem != null && _lastClickedItem != clickedItem)
            {
                _lastClickedItem.Reset();
            }

            _lastClickedItem = clickedItem;
            _lastClickedBorder = clickedBorder;
        }
        

        
    }
}
