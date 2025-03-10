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
        public ObservableCollection<string> SelectedIssues { get; set; } = new ObservableCollection<string>();
        

        public ICommand NavigateToTroubleshootingCommand { get; }

        public IssueSelectedViewModel()
        {
            SelectedIssues.Add("NetworkDrive");
            NavigateToTroubleshootingCommand = new RelayCommand(NavigateToTroubleshooting);
            
        }



        public void NavigateToTroubleshooting()
        {
            TroubleshootViewModel troubleshootViewModel = new TroubleshootViewModel(SelectedIssues);


            DetectingIssue detectingIssueView = new DetectingIssue();
            

            detectingIssueView.DataContext = troubleshootViewModel;
            detectingIssueView.Show();
            

        }
    }
}
