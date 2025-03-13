using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using WindowsTroubleShooter.Helpers;
using WindowsTroubleShooter.View;
using WindowsTroubleShooter.ViewModel;

namespace WindowsTroubleShooter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IUnityContainer _container;

        public App()
        {
            _container = new UnityContainer();

            // Register the Navigation Service
            _container.RegisterSingleton<INavigateService, NavigationService>();

            // Register ViewModels and Views
            _container.RegisterType<StartViewModel>();
            _container.RegisterType<TroubleshootViewModel>();

            // Start the app by showing the first view
            var mainWindow = _container.Resolve<StartView>(); // Assuming MainWindow has a constructor with the ViewModel injected.
            mainWindow.Show();
        }
    }
}
