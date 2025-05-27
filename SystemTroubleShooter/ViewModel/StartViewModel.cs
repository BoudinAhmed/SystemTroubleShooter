using System.Windows.Controls;
using System.Windows.Input;
using SystemTroubleShooter.Helpers.Commands;

namespace SystemTroubleShooter.ViewModel
{
    public class StartViewModel : ViewModelBase
    {

        //-Declarations
        public ICommand SwitchToHomeCommand { get; set; }
        public ICommand SwitchToSystemOverviewCommand { get; set; }
        public ICommand SwitchToSettingsCommand { get; set; }
        public ICommand SwitchToAboutCommand { get; set; }

        private IssueItemViewModel? _lastClickedItem;
        private Border? _lastClickedBorder;

        private string? _selectedView;

        public string? SelectedView
        {
            get => _selectedView;
            set => SetProperty(ref _selectedView, value);
        }


        private ViewModelBase? _currentContentViewModel;

        public ViewModelBase? CurrentContentViewModel
        {
            get { return _currentContentViewModel; }
            set
            {
                SetProperty(ref _currentContentViewModel, value);
                // Subscribe to event when switching to DashboardViewModel
                
            }
        }

        public StartViewModel() 
        {

            CurrentContentViewModel = new HomeViewModel();
            UpdateSelectedView(CurrentContentViewModel);

            //Navigation commands
            SwitchToHomeCommand = new RelayCommand(SwitchToHome);
            SwitchToSystemOverviewCommand = new RelayCommand(SwitchToSystemOverview);
            SwitchToSettingsCommand = new RelayCommand(SwitchToSettings);
            SwitchToAboutCommand = new RelayCommand(SwitchToAbout);

           
        
            

        }


        private void SwitchToHome(object? o)
        {
            if (CurrentContentViewModel is not HomeViewModel)
            {

                CurrentContentViewModel = new HomeViewModel();
                UpdateSelectedView(CurrentContentViewModel);

            }
        }

        private void SwitchToSystemOverview(object? o)
        {
            if (CurrentContentViewModel is not SystemOverviewViewModel)
            {

                CurrentContentViewModel = new SystemOverviewViewModel();
                UpdateSelectedView(CurrentContentViewModel);

            }
        }

        private void SwitchToSettings(object? o)
        {
            if (CurrentContentViewModel is not SettingsViewModel)
            {

                CurrentContentViewModel = new SettingsViewModel();
                UpdateSelectedView(CurrentContentViewModel);

            }
        }

        private void SwitchToAbout(object? o)
        {
            if (CurrentContentViewModel is not AboutViewModel)
            {

                CurrentContentViewModel = new AboutViewModel();
                UpdateSelectedView(CurrentContentViewModel);

            }
        }
        

        private void UpdateSelectedView(object currentControl)
        {
            if (currentControl is SystemOverviewViewModel)
            {
                SelectedView = "SystemOverview";
            }
            else if (currentControl is HomeViewModel)
            {
                SelectedView = "Home";
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
                _lastClickedItem.ResetIssueItem();
            }

            _lastClickedItem = clickedItem;
            _lastClickedBorder = clickedBorder;
        }
        

        
    }
}
