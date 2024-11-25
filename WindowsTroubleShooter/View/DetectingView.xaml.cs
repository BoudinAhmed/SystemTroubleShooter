using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WindowsTroubleShooter.ViewModel;

namespace WindowsTroubleShooter.View
{
    /// <summary>
    /// Interaction logic for DetectingIssue.xaml
    /// </summary>
    public partial class DetectingIssue : Window
    {
        public StartView startView { get; set; }
        public DetectingIssue(StartView startView)
        {
            InitializeComponent();
            this.startView = startView;
            
            
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => { TroubleshootSearchBar(); }));
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => { MapNetwordDrives(); }));
        }
        

        
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
     

        //This is where the logic for the troubleshooting goes

        //For the searchbar issue
        protected async void TroubleshootSearchBar()
        {
            SearchBarIssue searchBarIssue = new SearchBarIssue(this);

            //verifying if the registry contains the correct value
            if (startView.SearchIssueOption.IsChecked == true)
            {
                await searchBarIssue.verifiyRegistry();

                //Navigate to the Exit page
                ExitWindow exitWindow = new ExitWindow();
                exitWindow.Show();
                Close();
            }
        }

        //To map the drives
        protected async void MapNetwordDrives()
        {
            MissingDrives missingDrives = new MissingDrives(this);
            if (startView.MapDrives.IsChecked == true)
            {
              //replace letter and path per you internal network drive
              await missingDrives.MapNetworkDrive("Letter", @"\\path\");
              await missingDrives.MapNetworkDrive("Letter", @"\\path\");

                //Displaying to the user the task

              await Task.Delay(800);
              repairs.Content = "Mapping drives has been completed";
              await Task.Delay(500);
                ExitWindow exitWindow = new ExitWindow();
              exitWindow.Show();
              Close();
            }
        }
        


    }
}
