using Caliburn.Micro;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;

namespace WindowsTroubleShooter.ViewModel
{
    class IssueSelectedViewModel : ObservableObject
    {
        public ObservableCollection<string> SelectIssues { get; } = new();
        public ICommand StartTroubleshootingCommand { get; }


    }
}
