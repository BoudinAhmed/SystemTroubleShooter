using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemTroubleShooter.Interfaces;

namespace SystemTroubleShooter.Test.Mocks
{
    public class MockAudioDeviceDisplayService : IAudioDeviceDisplayService
    {
        public Task<string?> SelectAudioDeviceAsync(List<string> outputDevices, List<string> inputDevices)
        {
            return Task.FromResult<string?>("Mock Device");
        }

        void IAudioDeviceDisplayService.DisplayAudioDevices(List<string> outputDevices, List<string> inputDevices)
        {
            // To implement
        }
    }
}
