using Caliburn.Micro;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using WindowsTroubleShooter.Helpers;
using WindowsTroubleShooter.View;

namespace WindowsTroubleShooter.ViewModel
{
    public class IssueSelectedViewModel : ObservableObject
    {
        public ObservableCollection<string> SelectedIssues { get; } = new();
        public ICommand StartTroubleshootingCommand { get; }
        public ICommand NavigateToTroubleshootingCommand { get; }

        public IssueSelectedViewModel()
        {

            //StartTroubleshootingCommand = new RelayCommand(StartTroubleshooting);
            NavigateToTroubleshootingCommand = new RelayCommand(NavigateToTroubleshooting);
        }

        private void StartTroubleshooting()
        {

            TroubleshootViewModel issues = new TroubleshootViewModel(SelectedIssues);
            
        }

        public void NavigateToTroubleshooting()
        {
            DetectingIssue detectingIssue = new DetectingIssue();
            detectingIssue.Show();
            

        }
    }
}
