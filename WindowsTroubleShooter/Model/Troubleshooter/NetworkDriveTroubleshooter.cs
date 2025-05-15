using GalaSoft.MvvmLight.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WindowsTroubleShooter.ViewModel;

namespace WindowsTroubleShooter.Model.Troubleshooter
{
    public class NetworkDriveTroubleshooter : BaseTroubleshooter
    {
        private const string _mapNetworkDriveScriptPath = @"Scripts\\NetworkDrive\\MapNetworkDrive.ps1";
        private readonly string _settingsFilePath;
        private List<TroubleshootingStep> _troubleshootingSteps;

        private Dictionary<string, string> _configuredNetworkDrives;
        public Dictionary<string, string> ConfiguredNetworkDrives
        {
            get => _configuredNetworkDrives;
            set { _configuredNetworkDrives = value; }

        }

        public NetworkDriveTroubleshooter()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(localAppData, "WindowsTroubleShooter");
            Directory.CreateDirectory(appFolder); // Ensure the directory exists
            _settingsFilePath = Path.Combine(appFolder, "settings.json");

            GetNetworkDrivesFromCache();

            _troubleshootingSteps = new List<TroubleshootingStep>
            {
                new TroubleshootingStep {
                    Description = "Mapping Network Drive",
                    ScriptPath = _mapNetworkDriveScriptPath,
                    ScriptArguments = string.Join(";", ConfiguredNetworkDrives.Select(kvp => $"{kvp.Key},{kvp.Value}")),
                    IsCritical = false // If one fails, we can still try the next one
                }
            };

        }


        public virtual void GetNetworkDrivesFromCache()
        {
            if (File.Exists(_settingsFilePath))
            {
                try
                {
                    string jsonString = File.ReadAllText(_settingsFilePath);
                    var savedSettings = JsonSerializer.Deserialize<SettingsData>(jsonString);



                    if (savedSettings?.NetworkDrives != null || savedSettings?.NetworkDrives.Count > 0)
                        ConfiguredNetworkDrives = savedSettings.NetworkDrives ?? new Dictionary<string, string>();
                        
                    
                    else
                    {
                        ConfiguredNetworkDrives = new Dictionary<string, string>();
                        StatusMessage = "No network drives configured. Go to Settings";
                        Task.Delay(5000);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading settings: {ex.Message}");
                    ConfiguredNetworkDrives = new Dictionary<string, string>();
                    
                }
            }
            else
            {
                // Load default settings if the file doesn't exist
                ConfiguredNetworkDrives = new Dictionary<string, string>();
               
            }
            
        
        }

        public override async Task<string> RunDiagnosticsAsync()
        {

            string jsonString = File.ReadAllText(_settingsFilePath);

            foreach (var step in _troubleshootingSteps)
            {

                (IsFixed, ResolutionMessage) = await ExecuteTroubleshootingStepAsync(step);
              
                if (step.IsCritical && !IsFixed)
                   break;
                
            }
            return ResolutionMessage;

        }

    }
}
