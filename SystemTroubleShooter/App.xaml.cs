using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Unity;
using SystemTroubleShooter.Interfaces;
using SystemTroubleShooter.View;
using SystemTroubleShooter.ViewModel;
using SystemTroubleShooter.Helpers.Services;

namespace SystemTroubleShooter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            
            // Start the app with the first view
            var firstView = new StartView
            {
                DataContext = new StartViewModel()
            };
            firstView.Show();
        }
    }
}
