using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Unity;
using WindowsTroubleShooter.Helpers;
using WindowsTroubleShooter.Interfaces;
using WindowsTroubleShooter.View;
using WindowsTroubleShooter.ViewModel;

namespace WindowsTroubleShooter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static NavigationService NavigationService { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize the Navigation Service
            NavigationService = new NavigationService();

            // Start the app with the first view
            var firstView = new StartView
            {
                DataContext = new StartViewModel()
            };
            firstView.Show();
        }
    }
}
