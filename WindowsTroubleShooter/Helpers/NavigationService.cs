using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WindowsTroubleShooter.Helpers
{
    public class NavigationService : INavigateService
    {
        private readonly Func<Type, object> _viewModelFactory;

        public NavigationService(Func<Type, object> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public void NavigateTo<TViewModel>(ObservableCollection<string> selectedIssues)
        {
            // Create an instance of the ViewModel and pass the selectedIssues to the constructor
            var viewModel = (TViewModel)Activator.CreateInstance(typeof(TViewModel), selectedIssues);

            // Map ViewModel to a View (using the ViewModel's name, we assume the convention "ViewModel" -> "View")
            var viewType = typeof(TViewModel).Name.Replace("ViewModel", "View"); // Map to View
            var view = Activator.CreateInstance(Type.GetType($"WindowsTroubleShooter.View.{viewType}")) as Window;

            if (view != null)
            {
                // Bind the DataContext of the View to the ViewModel
                view.DataContext = viewModel;
                view.Show();
            }
        }
    }


}
