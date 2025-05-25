using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace SystemTroubleShooter.Model.Troubleshooter
{
    public class SoundTroubleshooter : BaseTroubleshooter
    {
        private const string _checkAudioServices = @"Scripts\\Sound\\Check-Restart-AudioServices.ps1";
        private const string _getAllAudioDevices = @"Scripts\\Sound\\GetAllAudioDevices.ps1";
        private readonly List<TroubleshootingStep> _troubleshootingSteps;
        
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

        public async Task<(List<string> InputDevices, List<string> OutputDevices)> GetAllAudioDevicesAsync()
        {
            List<string> inputDevices = new List<string>();
            List<string> outputDevices = new List<string>();

            // _getAllAudioDevices should be the full path to your updated PowerShell script file.
            var (standardOutput, standardError, exitCode) = await ExecutePowerShellScriptAsync(_getAllAudioDevices);

            if (exitCode == 0 && !string.IsNullOrWhiteSpace(standardOutput))
            {
                try
                {
                    // Deserialize the JSON string into your AudioDeviceLists object
                    var deviceLists = JsonSerializer.Deserialize<AudioDeviceLists>(standardOutput);

                    if (deviceLists != null)
                    {
                        // Assign the parsed lists, handling potential nulls from deserialization if properties weren't in JSON
                        inputDevices = deviceLists.InputDevices ?? new List<string>();
                        outputDevices = deviceLists.OutputDevices ?? new List<string>();
                    }
                }
                catch (JsonException ex)
                {
                    // Log JSON parsing errors. This is crucial if the PowerShell output isn't what's expected.
                    Debug.WriteLine($"JSON Deserialization Error in GetAllAudioDevicesSeparatedAsync: {ex.Message}");
                    Debug.WriteLine($"Raw PowerShell Output that caused error: {standardOutput}");
                }
            }
            else // Handle cases where PowerShell script failed or returned no output
            {
                Debug.WriteLine($"PowerShell script error or no output in GetAllAudioDevicesSeparatedAsync. Exit Code: {exitCode}");
                Debug.WriteLine($"PowerShell Error Output: {standardError}");
                // You might want to return empty lists or throw an exception here depending on desired error handling.
            }

            // Return the tuple of separated lists
            return (inputDevices, outputDevices);
        }



        public override async Task<string> RunDiagnosticsAsync()
        {
            (List<string> InputDevices, List<string> OutputDevices) = await GetAllAudioDevicesAsync();

            if (InputDevices.Any())
            {
                DetailedLog += "Available Audio Devices:" + Environment.NewLine;
                foreach (var deviceName in InputDevices)
                {
                    Debug.WriteLine(deviceName);
                    DetailedLog += $"- {deviceName}{Environment.NewLine}";
                }
            }
            else
            {
                DetailedLog += "No audio devices found via PowerShell script." + Environment.NewLine;
            }


            /*foreach (var step in _troubleshootingSteps)
            {
                (IsFixed, ResolutionMessage) = await ExecuteTroubleshootingStepAsync(step);
            }*/


            return "te";
        }
    }

    public class AudioDeviceLists
    {
        public List<string>? InputDevices { get; set; }
        public List<string>? OutputDevices { get; set; }
    }

}
