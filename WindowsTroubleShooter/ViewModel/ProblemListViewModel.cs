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
    public class ProblemListViewModel
    {
        public ObservableCollection<IssueItemViewModel> IssueItems { get; set; }
        private IssueItemViewModel _lastClickedItem;
        private Border _lastClickedBorder;
        public string Version { get; set; }
        public ObservableCollection<string> SelectedIssues { get; set; } = new ObservableCollection<string>();

        public ProblemListViewModel()
        { 
         IssueItems = new ObservableCollection<IssueItemViewModel>
            {
                new IssueItemViewModel
                {
                    Title = "Internet Connection",
                    Description = "Fix problems with connecting to the internet",
                    ImageSource = "\xE701",
                    IssueType = new InternetTroubleshooter()


                },
                new IssueItemViewModel
                {
                    Title = "Windows Update",
                    Description = "Resolve problem with windows update",
                    ImageSource = "\xE895",
                    IssueType = new InternetTroubleshooter() //To be changed
                },
                new IssueItemViewModel
                {
                    Title = "Sound",
                    Description = "Fix problems with playing audio",
                    ImageSource = "\xE767",
                    IssueType = new InternetTroubleshooter() //To be changed
                },
                new IssueItemViewModel
                {
                    Title = "Map Network Drive",
                    Description = "Map a network drive with letter and path",
                    ImageSource = "\xE8CE",
                    IssueType = new InternetTroubleshooter() //To be changed
                },
                new IssueItemViewModel
                {
                    Title = "Internet Connection",
                    Description = "Fix problems with connecting to the internet",
                    ImageSource = "\xE701",
                    IssueType = new InternetTroubleshooter()


                },
                new IssueItemViewModel
                {
                    Title = "Windows Update",
                    Description = "Resolve problem with windows update",
                    ImageSource = "\xE895",
                    IssueType = new InternetTroubleshooter() //To be changed
                },
                new IssueItemViewModel
                {
                    Title = "Sound",
                    Description = "Fix problems with playing audio",
                    ImageSource = "\xE767",
                    IssueType = new InternetTroubleshooter() //To be changed
                },
                new IssueItemViewModel
                {
                    Title = "Map Network Drive",
                    Description = "Map a network drive with letter and path",
                    ImageSource = "\xE8CE",
                    IssueType = new InternetTroubleshooter() //To be changed
                }

            };
         Version = "Version 2.0.1";
        }
    }
}
