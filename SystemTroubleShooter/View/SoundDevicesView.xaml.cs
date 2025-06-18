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


        // Optional: If you want to allow dragging the window (since WindowStyle="None")
        private void SoundDeviceWindow_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}
