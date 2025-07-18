﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Input;
using SystemTroubleShooter.Helpers.Commands;
using SystemTroubleShooter.Model.Troubleshooter;

namespace SystemTroubleShooter.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly string _settingsFilePath;
        private string? _savedDeviceName;

        // --- Properties Relevant to Network Drives ---
        private Dictionary<string, string> _configuredNetworkDrives;
        public Dictionary<string, string> ConfiguredNetworkDrives
        {
            get => _configuredNetworkDrives;
            set
            {
                if (SetProperty(ref _configuredNetworkDrives, value))
                {
                    SaveSettings();
                    UpdateAvailableDriveLetters();
                }
            }
        }

        private string? _newDriveLetter;
        public string? NewDriveLetter
        {
            get => _newDriveLetter;
            set => SetProperty(ref _newDriveLetter, value);
        }

        private string? _newDrivePath;
        public string? NewDrivePath
        {
            get => _newDrivePath;
            set => SetProperty(ref _newDrivePath, value);
        }

        private ObservableCollection<string> _availableDriveLetters;
        public ObservableCollection<string> AvailableDriveLetters
        {
            get => _availableDriveLetters;
            set => SetProperty(ref _availableDriveLetters, value);
        }

        // --- Properties Relevant to Internet Settings ---
        private string _preferredDns = "0.0.0.0";
        public string PreferredDns
        {
            get => _preferredDns;
            set => SetProperty(ref _preferredDns, value);
        }

        private string _alternateDns = "0.0.0.0";
        public string AlternateDns
        {
            get => _alternateDns;
            set => SetProperty(ref _alternateDns, value);
        }

        // --- Properties Relevant to Sound Settings ---

        private readonly SoundTroubleshooter _soundTroubleshooter;

        private ObservableCollection<OutputDevice> _availableOutputDevices;
        public ObservableCollection<OutputDevice> AvailableOutputDevices
        {
            get => _availableOutputDevices;
            set => SetProperty(ref _availableOutputDevices, value);
        }

        private OutputDevice? _selectedOutputDevice;
        public OutputDevice? SelectedOutputDevice
        {
            get => _selectedOutputDevice;
            set => SetProperty(ref _selectedOutputDevice, value);
        }

        // --- Properties Relevant to Windows Update Settings ---
        private bool _pauseUpdates;
        public bool PauseUpdates
        {
            get => _pauseUpdates;
            set => SetProperty(ref _pauseUpdates, value);
        }

        private string _activeHoursStart = "0.0.0.0";
        public string ActiveHoursStart
        {
            get => _activeHoursStart;
            set => SetProperty(ref _activeHoursStart, value);
        }

        private string _activeHoursEnd = "0.0.0.0";
        public string ActiveHoursEnd
        {
            get => _activeHoursEnd;
            set => SetProperty(ref _activeHoursEnd, value);
        }

        // --- Commands ---
        public ICommand SaveSettingsCommand { get; }
        public ICommand CancelSettingsCommand { get; }
        public ICommand AddNetworkDriveCommand { get; }
        public ICommand RemoveNetworkDriveCommand { get; }

        // --- Constructor & Methods ---
        public SettingsViewModel()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(localAppData, "WindowsTroubleShooter");
            Directory.CreateDirectory(appFolder); // Ensure the directory exists
            _settingsFilePath = Path.Combine(appFolder, "settings.json");

            // Initialize collections
            _soundTroubleshooter = new SoundTroubleshooter();
            _availableDriveLetters = new ObservableCollection<string>(GetInitialDriveLetters());
            _configuredNetworkDrives = new Dictionary<string, string>();
            _availableOutputDevices = new ObservableCollection<OutputDevice>(); 
           

            // Initialize Commands using RelayCommand
            SaveSettingsCommand = new RelayCommand(o => SaveSettings());
            CancelSettingsCommand = new RelayCommand(o => { LoadSettings(); LoadSoundDevicesAsync(); });
            AddNetworkDriveCommand = new RelayCommand(o => AddNetworkDrive(), o => CanAddNetworkDrive());
            RemoveNetworkDriveCommand = new RelayCommand(
                param => { if (param is string drivepath) 
                        RemoveNetworkDrive((drivepath)); }, // Execute action casts object to string
                param => param is string drivePath && CanRemoveNetworkDrive((string)param) // CanExecute predicate casts object to string
            );

            // Start loading process
            LoadSettings();
            LoadSoundDevicesAsync(); //LoadSettings() within method
        }

        private void AddNetworkDrive()
        {
            if (CanAddNetworkDrive())
            {
                if(NewDriveLetter is null || NewDrivePath is null) return;

                var updatedDrives = new Dictionary<string, string>(ConfiguredNetworkDrives);
                updatedDrives.Add(NewDriveLetter, NewDrivePath);
                ConfiguredNetworkDrives = updatedDrives;

                NewDriveLetter = "";
                NewDrivePath = string.Empty;
            }
        }

        private bool CanAddNetworkDrive()
        {
            return !string.IsNullOrWhiteSpace(NewDriveLetter) &&
                   AvailableDriveLetters.Contains(NewDriveLetter) &&
                   !string.IsNullOrWhiteSpace(NewDrivePath) &&
                   !ConfiguredNetworkDrives.ContainsKey(NewDriveLetter);
        }

        private void RemoveNetworkDrive(string driveLetterToRemove)
        {
            var updatedDrives = new Dictionary<string, string>(ConfiguredNetworkDrives);
            if (updatedDrives.Remove(driveLetterToRemove))
            {
                ConfiguredNetworkDrives = updatedDrives; // This will trigger SaveSettings and UpdateAvailableDriveLetters
            }
        }

        private bool CanRemoveNetworkDrive(string driveLetter)
        {
            return !string.IsNullOrEmpty(driveLetter) && ConfiguredNetworkDrives.ContainsKey(driveLetter);
        }

        public virtual void LoadSettings()
        {
            if (File.Exists(_settingsFilePath))
            {
                try
                {
                    string jsonString = File.ReadAllText(_settingsFilePath);
                    var savedSettings = JsonSerializer.Deserialize<SettingsData>(jsonString);
                    if (savedSettings != null)
                    {


                        _configuredNetworkDrives = savedSettings.NetworkDrives ?? new Dictionary<string, string>();

                        PreferredDns = savedSettings.PreferredDns ?? "0.0.0.0";
                        AlternateDns = savedSettings.AlternateDns ?? "0.0.0.0";
                        PauseUpdates = savedSettings.PauseUpdates;
                        ActiveHoursStart = savedSettings.ActiveHoursStart ?? "00:00";
                        ActiveHoursEnd = savedSettings.ActiveHoursEnd ?? "00:00";
                        _savedDeviceName = savedSettings.SelectedOutputDeviceName;
                        UpdateAvailableDriveLetters();
                    }
                    else
                    {
                        SetDefaultSettings();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading settings: {ex.Message}");
                    SetDefaultSettings();


                }
            }
            else
            {
                // Load default settings if the file doesn't exist
                SetDefaultSettings();
            }
        }

        private void SetDefaultSettings()
        {
            _configuredNetworkDrives = new Dictionary<string, string>();
            PreferredDns = "0.0.0.0";
            AlternateDns = "0.0.0.0";
            PauseUpdates = false;
            ActiveHoursStart = "09:00";
            ActiveHoursEnd = "17:00";
            _savedDeviceName = null;
            SelectedOutputDevice = null;
        }
        public virtual void SaveSettings()
        {
            try
            {
                var settingsToSave = new SettingsData
                {
                    NetworkDrives = ConfiguredNetworkDrives,
                    PreferredDns = PreferredDns,
                    AlternateDns = AlternateDns,
                    PauseUpdates = PauseUpdates,
                    ActiveHoursStart = ActiveHoursStart,
                    ActiveHoursEnd = ActiveHoursEnd,
                    SelectedOutputDeviceName = SelectedOutputDevice?.DeviceName
                    
                };
                string jsonString = JsonSerializer.Serialize(settingsToSave, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsFilePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        // Load output sound devices 
        private async void LoadSoundDevicesAsync()
        {
            var outputDevices = await _soundTroubleshooter.GetOutputDevicesAsync();


            AvailableOutputDevices.Clear();
            foreach (var device in outputDevices)
            {
                AvailableOutputDevices.Add(device);
            }

            if (!string.IsNullOrEmpty(_savedDeviceName))
            {
                SelectedOutputDevice = AvailableOutputDevices.FirstOrDefault(d => d.DeviceName == _savedDeviceName);
            }

            if (SelectedOutputDevice == null)
            {
                SelectedOutputDevice = AvailableOutputDevices.FirstOrDefault();
            }
        }

        // --- Helper Methods ---
        private void UpdateAvailableDriveLetters()
        {
            var initialLetters = GetInitialDriveLetters();
            var usedLetters = ConfiguredNetworkDrives.Keys;
            var available = initialLetters.Except(usedLetters).OrderBy(l => l);

            AvailableDriveLetters.Clear();
            foreach (var letter in available)
            {
                AvailableDriveLetters.Add(letter);
            }
        }

        private List<string> GetInitialDriveLetters()
        {
            return Enumerable.Range('C', 'Z' - 'C' + 1).Select(i => (char)i + ":").ToList();
        }
    }


    // Model for SettingsData 
    public class SettingsData
    {
        public string? PreferredDns { get; set; }
        public string? AlternateDns { get; set; }
        public Dictionary<string, string>? NetworkDrives { get; set; }
        public bool PauseUpdates { get; set; }
        public string? ActiveHoursStart { get; set; }
        public string? ActiveHoursEnd { get; set; }
        public string? SelectedOutputDeviceName { get; set; } 
    }

    // Model for OutputDevice 
    public class OutputDevice
    {
        public string? DeviceName { get; set; }
        // other properties to be added
    }
}