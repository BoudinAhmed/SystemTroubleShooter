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

        //-Declarations
        public ICommand SwitchToDashboardCommand { get; set; }
        public ICommand SwitchToProblemListCommand { get; set; }
        public ICommand SwitchToSettingsCommand { get; set; }
        private IssueItemViewModel _lastClickedItem;
        private Border _lastClickedBorder;

        private string _selectedView;

        public string SelectedView
        {
            get => _selectedView;
            set => SetProperty(ref _selectedView, value);
        }


        private UserControl _currentUserControl = new DashboardView();

        public UserControl CurrentUserControl
        {
            get { return _currentUserControl; }
            set
            {
                SetProperty(ref _currentUserControl, value);
                // Subscribe to the event whenever CurrentUserControl becomes a DashboardView
                if (_currentUserControl is DashboardView dashboardView && dashboardView.DataContext is DashboardViewModel dashboardViewModel)
                {
                    dashboardViewModel.RequestNavigateToProblemList -= DashboardViewModel_RequestNavigateToProblemList; // Unsubscribe first to avoid multiple subscriptions
                    dashboardViewModel.RequestNavigateToProblemList += DashboardViewModel_RequestNavigateToProblemList;

                }
            }
        } 

        public StartViewModel() 
        {

            _currentUserControl = new DashboardView();
            UpdateSelectedView(_currentUserControl);

            //Navigation commands
            SwitchToDashboardCommand = new RelayCommand(SwitchToDashboard);
            SwitchToProblemListCommand = new RelayCommand(SwitchToProblemList);
            SwitchToSettingsCommand = new RelayCommand(SwitchToSettings);
            
            if (CurrentUserControl is DashboardView dashboardView)
        {
            if (dashboardView.DataContext is DashboardViewModel dashboardViewModel)
            {
                dashboardViewModel.RequestNavigateToProblemList += DashboardViewModel_RequestNavigateToProblemList;
            }
        }

        }

        private void SwitchToSettings()
        {
            if (CurrentUserControl is not SettingsView)
            {
                FadeOut(() =>
                {
                    CurrentUserControl = new SettingsView();
                    UpdateSelectedView(CurrentUserControl);
                    FadeIn();
                });

            }
        }

        private void SwitchToDashboard()
        {
            if (CurrentUserControl is not DashboardView)
            {
                FadeOut(() =>
            {
                CurrentUserControl = new DashboardView();
                UpdateSelectedView(CurrentUserControl);
                FadeIn();
            });

            }
        }

        private void SwitchToProblemList()
        {
            if (CurrentUserControl is not ProblemListView)
            {
                
                    CurrentUserControl = new ProblemListView();
                    UpdateSelectedView(CurrentUserControl);
                    
            }
        }

        private void DashboardViewModel_RequestNavigateToProblemList(object sender, EventArgs e)
        {
            CurrentUserControl = new ProblemListView();
            UpdateSelectedView(CurrentUserControl);
        }

        private void UpdateSelectedView(UserControl currentControl)
        {
            if (currentControl is DashboardView)
            {
                SelectedView = "Dashboard";
            }
            else if (currentControl is ProblemListView)
            {
                SelectedView = "ProblemList";
            }
            else if (currentControl is SettingsView)
            {
                SelectedView = "Settings";
            }
            else
            {
                SelectedView = null; // Or some default
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
