using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SystemTroubleShooter.Interfaces;
using SystemTroubleShooter.View;
using SystemTroubleShooter.ViewModel;

namespace SystemTroubleShooter.Helpers
{
    public class AudioDeviceDisplayService : IAudioDeviceDisplayService
    {
       

        public void DisplayAudioDevices(List<string> outputDevices, List<string> inputDevices)
        {
            var audioDeviceView = new SoundDevicesView();
            audioDeviceView.DataContext = new SoundDevicesViewModel(outputDevices, inputDevices);
            audioDeviceView.Show();
           
        }
    }
}
