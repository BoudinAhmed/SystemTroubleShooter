using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class TroubleshootView : Window
    {
        public TroubleshootView() 
        { 
            InitializeComponent(); 
        }


        public TroubleshootView(ObservableCollection<string> selectedIssues)
        {
            InitializeComponent();
            this.DataContext = new TroubleshootViewModel(selectedIssues);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
            
        }



    



    }
}
