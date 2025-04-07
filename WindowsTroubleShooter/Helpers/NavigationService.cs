using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WindowsTroubleShooter.Interfaces;
using WindowsTroubleShooter.Model;

namespace WindowsTroubleShooter.Helpers
{
    public class NavigationService
    {
        public void NavigateTo<TView, TViewModel>(TViewModel viewModel)
            where TView : Window, new()
            where TViewModel : class
        {
            var view = new TView
            {
                DataContext = viewModel
            };

            view.Show();
        }
    }


}
