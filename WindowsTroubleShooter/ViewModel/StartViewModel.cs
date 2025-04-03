using Caliburn.Micro;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using WindowsTroubleShooter.View;
using WindowsTroubleShooter.Interfaces;
using WindowsTroubleShooter.Helpers.Commands;

namespace WindowsTroubleShooter.ViewModel
{
    public class StartViewModel : ObservableObject
    {
        private readonly INavigateService _navigationService;

        public ObservableCollection<string> SelectedIssues { get; set; } = new ObservableCollection<string>();

        public ICommand NavigateToTroubleshootingCommand { get; }

        public StartViewModel() { }

        // Constructor injection for the navigation service
        public StartViewModel(INavigateService navigationService)
        {
            _navigationService = navigationService;
            NavigateToTroubleshootingCommand = new RelayCommand(NavigateToTroubleshooting);

<<<<<<< HEAD
=======
            
>>>>>>> 80f229745252eb5d7a29544152d0eca4a156ae5c
        }

        public void NavigateToTroubleshooting()
        {
            // Call NavigateTo on the navigation service and pass SelectedIssues
            _navigationService.NavigateTo<TroubleshootViewModel>(SelectedIssues);
        }
    }
}
