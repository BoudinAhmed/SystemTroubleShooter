using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsTroubleShooter.Interfaces
{
    public interface INavigateService
    {
        void NavigateTo<TViewModel>(System.Collections.ObjectModel.ObservableCollection<string> selectedIssues);
    }
}
