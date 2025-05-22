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
        private const string _checkAudioServices = @"Scripts\\Sound\\Check-Restart-AudioServices.ps1";
        private const string _getAllAudioDevices = @"Scripts\\Sound\\GetAllAudioDevices.ps1";
        private readonly List<TroubleshootingStep> _troubleshootingSteps;
        private readonly string _preferredAudioDevice; 
        public SoundTroubleshooter() 
        {
            

            _troubleshootingSteps = new List<TroubleshootingStep>() 
            {
                new() {
                    Description = "Checking Audio Services",
                    ScriptPath = _checkAudioServices,
                    IsCritical = true 
                }, 
            };
        }

        public async Task<List<string>> GetAllAudioDevicesAsync()
        {
            List<string> devices = new List<string>();
            // Execute the script and get its output
            var (standardOutput, standardError, exitCode) = await ExecutePowerShellScriptAsync(_getAllAudioDevices);

            if (exitCode == 0 && !string.IsNullOrWhiteSpace(standardOutput))
            {
                // Split the output by new lines.
                // Remove empty entries which might occur due to extra newlines.
                devices = standardOutput.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(device => device.Trim()) // Trim whitespace from each device name
                                        .ToList();
            }
            else if (exitCode != 0)
            {
                // Log the error or handle it as appropriate
                Debug.WriteLine($"Error getting audio devices. Exit Code: {exitCode}");
                Debug.WriteLine($"Error Output: {standardError}");
                // Optionally, you could throw an exception or return an empty list with a status message.
            }
            return devices;
        }

        public static float GetMasterVolumeNAudio()
        {
            // Releases COM objects correctly and to ensure Dispose is called
            using var enumerator = new MMDeviceEnumerator();
            using var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            if (device != null && device.AudioEndpointVolume != null)
            {
                float volume = device.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
                return volume;
            }
            return -1f;
        }

        public static bool IsMasterVolumeMutedNAudio()
        {

            // Releases COM objects correctly and to ensure Dispose is called
            using var enumerator = new MMDeviceEnumerator();
            using var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            if (device != null && device.AudioEndpointVolume != null)
            {
                bool muted = device.AudioEndpointVolume.Mute;
                return muted;
            }
            // Todo: Handle device or volume control is not available



            throw new InvalidOperationException("Could not retrieve default audio device or its volume control.");
        }


        public override async Task<string> RunDiagnosticsAsync()
        {
            List<string> audioDevices = await GetAllAudioDevicesAsync();
            if (audioDevices.Any())
            {
                DetailedLog += "Available Audio Devices:" + Environment.NewLine;
                foreach (var deviceName in audioDevices)
                {
                    Debug.WriteLine(deviceName);
                    DetailedLog += $"- {deviceName}{Environment.NewLine}";
                }
            }
            else
            {
                DetailedLog += "No audio devices found via PowerShell script." + Environment.NewLine;
            }


            foreach (var step in _troubleshootingSteps)
            {
                (IsFixed, ResolutionMessage) = await ExecuteTroubleshootingStepAsync(step);
            }


            return "te";
        }
    }
}
