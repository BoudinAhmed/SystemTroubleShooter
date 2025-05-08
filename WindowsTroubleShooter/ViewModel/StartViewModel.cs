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
using System.Diagnostics;

namespace WindowsTroubleShooter.ViewModel
{
    public class StartViewModel : ViewModelBase
    {

        //-Declarations
        public ICommand SwitchToDashboardCommand { get; set; }
        public ICommand SwitchToProblemListCommand { get; set; }
        public ICommand SwitchToSettingsCommand { get; set; }
        public ICommand SwitchToAboutCommand { get; set; }

        private IssueItemViewModel _lastClickedItem;
        private Border _lastClickedBorder;

        private string _selectedView;

        public string SelectedView
        {
            get => _selectedView;
            set => SetProperty(ref _selectedView, value);
        }


        private ViewModelBase _currentContentViewModel = new DashboardViewModel();

        public ViewModelBase CurrentContentViewModel
        {
            get { return _currentContentViewModel; }
            set
            {
                SetProperty(ref _currentContentViewModel, value);
                // Subscribe to event when switching to DashboardViewModel
                if (_currentContentViewModel is DashboardViewModel dashboardViewModel)
                {
                    dashboardViewModel.RequestNavigateToProblemList -= DashboardViewModel_RequestNavigateToProblemList;
                    dashboardViewModel.RequestNavigateToProblemList += DashboardViewModel_RequestNavigateToProblemList;
                }
            }
        }

        public StartViewModel() 
        {

            _currentContentViewModel = new DashboardViewModel();
            UpdateSelectedView(_currentContentViewModel);

            //Navigation commands
            SwitchToDashboardCommand = new RelayCommand(SwitchToDashboard);
            SwitchToProblemListCommand = new RelayCommand(SwitchToProblemList);
            SwitchToSettingsCommand = new RelayCommand(SwitchToSettings);
            SwitchToAboutCommand = new RelayCommand(SwitchToAbout);

            if (CurrentContentViewModel is DashboardViewModel dashboardViewModel)
            {
            
                dashboardViewModel.RequestNavigateToProblemList += DashboardViewModel_RequestNavigateToProblemList;
            }
        
            

        }


        private void SwitchToDashboard()
        {
            if (CurrentContentViewModel is not DashboardViewModel)
            {

                CurrentContentViewModel = new DashboardViewModel();
                UpdateSelectedView(CurrentContentViewModel);

            }
        }

        private void SwitchToProblemList()
        {
            if (CurrentContentViewModel is not ProblemListViewModel)
            {
                
                    CurrentContentViewModel = new ProblemListViewModel();
                    UpdateSelectedView(CurrentContentViewModel);
                    
            }
        }
        private void SwitchToSettings()
        {
            if (CurrentContentViewModel is not SettingsViewModel)
            {

                CurrentContentViewModel = new SettingsViewModel();
                UpdateSelectedView(CurrentContentViewModel);

            }
        }

        private void SwitchToAbout()
        {
            if (CurrentContentViewModel is not AboutViewModel)
            {

                CurrentContentViewModel = new AboutViewModel();
                UpdateSelectedView(CurrentContentViewModel);

            }
        }
        private void DashboardViewModel_RequestNavigateToProblemList(object sender, EventArgs e)
        {
            CurrentContentViewModel = new ProblemListViewModel();
            UpdateSelectedView(CurrentContentViewModel);
        }

        private void UpdateSelectedView(object currentControl)
        {
            if (currentControl is DashboardViewModel)
            {
                SelectedView = "Dashboard";
            }
            else if (currentControl is ProblemListViewModel)
            {
                SelectedView = "ProblemList";
            }
            else if (currentControl is SettingsViewModel)
            {
                SelectedView = "Settings";
            }
            else if (currentControl is AboutViewModel)
            {
                SelectedView = "About";
            }
            else
            {
                SelectedView = null; // Or some default
            }
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
