using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemTroubleShooter.Interfaces
{
    public interface IAudioDeviceDisplayService
    {
        void DisplayAudioDevices(List<string> outputDevices, List<string> inputDevices);
        Task<string?> SelectAudioDeviceAsync(List<string> outputDevices, List<string> inputDevices);
    }
}
