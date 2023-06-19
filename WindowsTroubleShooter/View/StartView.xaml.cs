using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WindowsTroubleShooter.View
{
    /// <summary>
    /// Interaction logic for StartView.xaml
    /// </summary>
    public partial class StartView : Window
    {
        public StartView()
        {
            InitializeComponent();

        }
        
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) { DragMove(); }
        }
        
        private void Button_Next(object sender, RoutedEventArgs e)
        {

            if (SearchIssueOption.IsChecked == true || MapDrives.IsChecked == true || UnlockAccount.IsChecked == true)
            {
                DetectingIssue detectingIssue = new DetectingIssue(this);
                detectingIssue.Show();
                Close();
            }
            
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        


        }
    }
