using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WindowsTroubleShooter.Interfaces;

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
            // Create ViewModel and pass the selectedIssues
            var viewModel = (TViewModel)Activator.CreateInstance(typeof(TViewModel), selectedIssues);

            // Map ViewModel to View
            var viewType = typeof(TViewModel).Name.Replace("ViewModel", "View"); // Map to View

            // Ensuring correct namespace for the View
            var viewFullTypeName = $"WindowsTroubleShooter.View.{viewType}";
            var viewTypeInstance = Type.GetType(viewFullTypeName);

            if (viewTypeInstance != null)
            {
                // If the View has a constructor that accepts parameters, we pass selectedIssues
                var constructor = viewTypeInstance.GetConstructor(new[] { typeof(ObservableCollection<string>) });

                if (constructor != null)
                {
                    var view = constructor.Invoke(new object[] { selectedIssues }) as Window;
                    if (view != null)
                    {
                        view.DataContext = viewModel;
                        view.Show();
                    }
                    else
                    {
                        Console.WriteLine($"Error: Could not create view of type {viewFullTypeName}");
                    }
                }
                else
                {
                    Console.WriteLine($"Error: No constructor found in {viewFullTypeName} that accepts ObservableCollection<string>");
                }
            }
            else
            {
                Console.WriteLine($"Error: Could not find type {viewFullTypeName}");
            }
        }
    }


}
