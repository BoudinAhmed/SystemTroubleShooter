using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WindowsTroubleShooter.Model;
using WindowsTroubleShooter.ViewModel;

namespace WindowsTroubleShooter.ViewModel
{
    public class ProblemListViewModel : ViewModelBase
    {

        //Declaration
        private string _version;
        private IssueItemViewModel _lastClickedItem;
        private Border _lastClickedBorder;
        private ObservableCollection<IssueItemViewModel> _issueItems;


        // --- Public Properties Binding --- //

        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }

        public ObservableCollection<IssueItemViewModel> IssueItems
        {
            get => _issueItems;
            set => SetProperty(ref _issueItems, value);
        }


        public ProblemListViewModel()
        {
            LoadVersionInfo();
            LoadIssueItems();
            
        }

        private void LoadIssueItems()
        {
            //Hardcoded for now will figure something out
            IssueItems.Add(new IssueItemViewModel { Title = "Internet Connection", Description = "Fix problems with connecting to the internet", ImageSource = "\xE701", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Windows Update",      Description = "Resolve problem with windows update",          ImageSource = "\xE895", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Sound",               Description = "Fix problems with playing audio",              ImageSource = "\xE767", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Map Network Drive",   Description = "Map a network drive with letter and path",     ImageSource = "\xE8CE", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Internet Connection", Description = "Fix problems with connecting to the internet", ImageSource = "\xE701", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Windows Update",      Description = "Resolve problem with windows update",          ImageSource = "\xE895", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Sound",               Description = "Fix problems with playing audio",              ImageSource = "\xE767", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Map Network Drive",   Description = "Map a network drive with letter and path",     ImageSource = "\xE8CE", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Internet Connection", Description = "Fix problems with connecting to the internet", ImageSource = "\xE701", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Windows Update",      Description = "Resolve problem with windows update",          ImageSource = "\xE895", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Sound",               Description = "Fix problems with playing audio",              ImageSource = "\xE767", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Map Network Drive",   Description = "Map a network drive with letter and path",     ImageSource = "\xE8CE", IssueType = new InternetTroubleshooter() });
        }

        private void LoadVersionInfo()
        {
            // Hardcoded for now but will load it from assembly info
            Version = "Version 2.0.1";
        }
    }
}
