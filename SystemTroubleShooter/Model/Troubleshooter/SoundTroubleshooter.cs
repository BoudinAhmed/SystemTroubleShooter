using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;

namespace SystemTroubleShooter.Model.Troubleshooter
{
    public class SoundTroubleshooter : BaseTroubleshooter
    {
        public static float GetMasterVolumeNAudio()
        {
            // Releases COM objects correctly and to ensure Dispose is called
            using (var enumerator = new MMDeviceEnumerator())
            {
                using (var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia))
                {
                    if (device != null && device.AudioEndpointVolume != null)
                    {
                        float volume = device.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
                        return volume;
                    }
                    return -1f; 
                }
            }
        }

        public static bool IsMasterVolumeMutedNAudio()
        {
            // Releases COM objects correctly and to ensure Dispose is called
            using (var enumerator = new MMDeviceEnumerator())
            {
                using (var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia))
                {
                    if (device != null && device.AudioEndpointVolume != null)
                    {
                        bool muted = device.AudioEndpointVolume.Mute;
                        return muted;
                    }
                    // Todo: Handle device or volume control is not available
                    
                    

                    throw new InvalidOperationException("Could not retrieve default audio device or its volume control.");
                }
            }
        }


        public override async Task<string> RunDiagnosticsAsync()
        {

            
            
            return "te";
        }
    }
}
