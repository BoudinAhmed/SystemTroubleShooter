using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemTroubleShooter.Model.Troubleshooter;

namespace SystemTroubleShooter.Interfaces
{
    public interface INavigateService
    {
        //void NavigateTo<TViewModel>();
        void NavigateTo<TViewModel>(BaseTroubleshooter selectedIssue);
    }
}
