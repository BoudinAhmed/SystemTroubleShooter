using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsTroubleShooter.Model.Troubleshooter;

namespace WindowsTroubleShooter.Interfaces
{
    public interface INavigateService
    {
        //void NavigateTo<TViewModel>();
        void NavigateTo<TViewModel>(BaseTroubleshooter selectedIssue);
    }
}
