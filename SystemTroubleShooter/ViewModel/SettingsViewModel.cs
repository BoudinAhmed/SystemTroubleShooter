using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Input;
using SystemTroubleShooter.Helpers.Commands;

namespace SystemTroubleShooter.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly string _settingsFilePath;

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
        private string _preferredDns;
        public string PreferredDns
        {
            get => _preferredDns;
            set => SetProperty(ref _preferredDns, value);
        }

        private string _alternateDns;
        public string AlternateDns
        {
            get => _alternateDns;
            set => SetProperty(ref _alternateDns, value);
        }

        // --- Properties Relevant to Sound Settings ---
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

        private string _activeHoursStart;
        public string ActiveHoursStart
        {
            get => _activeHoursStart;
            set => SetProperty(ref _activeHoursStart, value);
        }

        private string _activeHoursEnd;
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
            _availableDriveLetters = new ObservableCollection<string>(GetInitialDriveLetters());
            _configuredNetworkDrives = new Dictionary<string, string>();
            _availableOutputDevices = new ObservableCollection<OutputDevice>(); // Will populate this

            // Load existing settings when the ViewModel is created
            LoadSettings();

            // Initialize Commands using RelayCommand
            SaveSettingsCommand = new RelayCommand(o => SaveSettings());
            CancelSettingsCommand = new RelayCommand(o => LoadSettings()); // Cancel reloads last saved state
            AddNetworkDriveCommand = new RelayCommand(o => AddNetworkDrive(), o => CanAddNetworkDrive());
            RemoveNetworkDriveCommand = new RelayCommand(
                param => { if (param is string drivepath) 
                        RemoveNetworkDrive((drivepath)); }, // Execute action casts object to string
                param => param is string drivePath && CanRemoveNetworkDrive((string)param) // CanExecute predicate casts object to string
            );

            // Initialize other settings properties with default values / load them
            _preferredDns = string.Empty;
            _alternateDns = string.Empty;
            _pauseUpdates = false;
            _activeHoursStart = "09:00"; // Default start time
            _activeHoursEnd = "17:00";    // Default end time

            // Todo: implement how AvailableOutputDevices are fetched
            // ex _availableOutputDevices = GetAvailableSoundDevices();
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
                        
                      
                        ConfiguredNetworkDrives = savedSettings.NetworkDrives ?? new Dictionary<string, string>();

                        PreferredDns = savedSettings.PreferredDns ?? "0.0.0.0";
                        AlternateDns = savedSettings.AlternateDns ?? "0.0.0.0";
                        PauseUpdates = savedSettings.PauseUpdates;
                        ActiveHoursStart = savedSettings.ActiveHoursStart ?? "00:00";
                        ActiveHoursEnd = savedSettings.ActiveHoursEnd ?? "00:00";
                        // Todo: load SelectedOutputDevice based on some identifier
                        // For example, if SettingsData stores the DeviceName:
                        // SelectedOutputDevice = AvailableOutputDevices.FirstOrDefault(d => d.DeviceName == savedSettings.SelectedOutputDeviceName);
                        UpdateAvailableDriveLetters();
                    }
                    else
                    {
                        ConfiguredNetworkDrives = new Dictionary<string, string>();
                        UpdateAvailableDriveLetters();
                        // To initialize other properties to default values
                        PreferredDns = "0.0.0.0";
                        AlternateDns = "0.0.0.0";
                        PauseUpdates = false;
                        ActiveHoursStart = "09:00";
                        ActiveHoursEnd = "17:00";
                        SelectedOutputDevice = AvailableOutputDevices.FirstOrDefault() ?? new OutputDevice { DeviceName = string.Empty };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading settings: {ex.Message}");
                    ConfiguredNetworkDrives = new Dictionary<string, string>();
                    UpdateAvailableDriveLetters();
                    // To initialize other properties to default values on error
                    PreferredDns = string.Empty;
                    AlternateDns = string.Empty;
                    PauseUpdates = false;
                    ActiveHoursStart = "09:00";
                    ActiveHoursEnd = "17:00";
                    // Fix 3: Assign a default OutputDevice instead of null
                    SelectedOutputDevice = AvailableOutputDevices.FirstOrDefault() ?? new OutputDevice { DeviceName = string.Empty };
                }
            }
            else
            {
                // Load default settings if the file doesn't exist
                ConfiguredNetworkDrives = new Dictionary<string, string>();
                UpdateAvailableDriveLetters();
                PreferredDns = string.Empty;
                AlternateDns = string.Empty;
                PauseUpdates = false;
                ActiveHoursStart = "09:00";
                ActiveHoursEnd = "17:00";
                // Fix 4: Assign a default OutputDevice instead of null
                SelectedOutputDevice = AvailableOutputDevices.FirstOrDefault() ?? new OutputDevice { DeviceName = string.Empty };
            }
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
                    // Will decide how to store the selected output device
                    // Maybe store the DeviceName:
                    // SelectedOutputDeviceName = SelectedOutputDevice?.DeviceName
                };
                string jsonString = JsonSerializer.Serialize(settingsToSave, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsFilePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
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
        // public string SelectedOutputDeviceName { get; set; } // If I choose to store by name
    }

    // Model for OutputDevice 
    public class OutputDevice
    {
        public string? DeviceName { get; set; }
        // other properties to be added
    }
}