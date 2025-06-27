using System.Windows;
using System.Windows.Input;

namespace SystemTroubleShooter.View
{
    /// <summary>
    /// Interaction logic for SoundDevicesView.xaml
    /// </summary>
    public partial class SoundDevicesView : Window
    {
        public SoundDevicesView()
        {
            InitializeComponent();
        }
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
