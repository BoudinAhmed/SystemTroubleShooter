using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using WindowsTroubleShooter.Helpers.Commands;


namespace WindowsTroubleShooter.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {


            private readonly string _settingsFilePath;

            // --- Properties Relevant to Network Drives ---
            private Dictionary<string, string> _configuredNetworkDrives;
            public Dictionary<string, string> ConfiguredNetworkDrives
            {
                get => _configuredNetworkDrives;
                // Use your SetProperty method
                set => SetProperty(ref _configuredNetworkDrives, value);
            }

            private string _newDriveLetter;
            public string NewDriveLetter
            {
                get => _newDriveLetter;
                // Use your SetProperty method
                set => SetProperty(ref _newDriveLetter, value);
            }

            private string _newDrivePath;
            public string NewDrivePath
            {
                get => _newDrivePath;
                // Use your SetProperty method
                set => SetProperty(ref _newDrivePath, value);
            }

            // Using ObservableCollection for AvailableDriveLetters
            private ObservableCollection<string> _availableDriveLetters;
            public ObservableCollection<string> AvailableDriveLetters
            {
                get => _availableDriveLetters;
                set => SetProperty(ref _availableDriveLetters, value);
            }


            // --- Commands ---
            public ICommand SaveSettingsCommand { get; }
            public ICommand CancelSettingsCommand { get; }
            public ICommand AddNetworkDriveCommand { get; }
            public ICommand RemoveNetworkDriveCommand { get; }


            // --- Constructor & Methods ---

            // Field for Option 2 file path - declare if using Option 2
            // private readonly string _networkDrivesSettingsFilePath;

            public SettingsViewModel()
            {

            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(localAppData, "WindowsTroubleShooter");
            Directory.CreateDirectory(appFolder); // Ensure the directory exists
            _settingsFilePath = Path.Combine(appFolder, "settings.json");

            LoadSettings();
            // Initialize collections
            _availableDriveLetters = new ObservableCollection<string>(); // Initialize field directly
                _configuredNetworkDrives = new Dictionary<string, string>(); // Initialize field directly

                // Load existing settings when the ViewModel is created
                LoadSettings(); // This will populate ConfiguredNetworkDrives and update AvailableDriveLetters

                // Initialize Commands using your RelayCommand implementation
                // Use lambdas ignoring the 'object' parameter for commands that don't use it.
                SaveSettingsCommand = new RelayCommand(o => SaveSettings());
                CancelSettingsCommand = new RelayCommand(o => LoadSettings()); // Cancel reloads last saved state
                AddNetworkDriveCommand = new RelayCommand(o => AddNetworkDrive(), o => CanAddNetworkDrive());

                // For commands passing a parameter, cast the object parameter inside the lambda.
                RemoveNetworkDriveCommand = new RelayCommand(
                    param => RemoveNetworkDrive((string)param), // Execute action casts object to string
                    param => CanRemoveNetworkDrive((string)param) // CanExecute predicate casts object to string
                );
            }

            private void AddNetworkDrive()
            {
                // Create a *new* dictionary instance based on the old one
                var updatedDrives = new Dictionary<string, string>(ConfiguredNetworkDrives);
                updatedDrives.Add(NewDriveLetter, NewDrivePath);
                ConfiguredNetworkDrives = updatedDrives; // Triggers SetProperty -> OnPropertyChanged

                // Update available letters (remove the added one)
                AvailableDriveLetters.Remove(NewDriveLetter);

                // Clear input fields
                NewDriveLetter = null; // Triggers SetProperty
                NewDrivePath = string.Empty; // Triggers SetProperty
            }

            private bool CanAddNetworkDrive()
            {
                return !string.IsNullOrWhiteSpace(NewDriveLetter) &&
                       AvailableDriveLetters.Contains(NewDriveLetter) && // Ensure it's a valid available letter
                       !string.IsNullOrWhiteSpace(NewDrivePath) &&
                       !ConfiguredNetworkDrives.ContainsKey(NewDriveLetter);
            }

            private void RemoveNetworkDrive(string driveLetterToRemove)
            {
                // Create a *new* dictionary instance
                var updatedDrives = new Dictionary<string, string>(ConfiguredNetworkDrives);
                if (updatedDrives.Remove(driveLetterToRemove)) // Only proceed if removal was successful
                {
                    ConfiguredNetworkDrives = updatedDrives; // Triggers SetProperty -> OnPropertyChanged

                    // Add the letter back to available list if needed and not already present
                    if (!AvailableDriveLetters.Contains(driveLetterToRemove))
                    {
                        AvailableDriveLetters.Add(driveLetterToRemove);
                        // Re-sort the list if desired (ObservableCollection doesn't sort automatically)
                        var sortedLetters = AvailableDriveLetters.OrderBy(l => l).ToList();
                        AvailableDriveLetters.Clear();
                        foreach (var l in sortedLetters) AvailableDriveLetters.Add(l);
                    }
                }
            }

            private bool CanRemoveNetworkDrive(string driveLetter)
            {
                // Can remove if the parameter is a valid key in the current dictionary
                return !string.IsNullOrEmpty(driveLetter) && ConfiguredNetworkDrives.ContainsKey(driveLetter);
            }


            // --- Load/Save Methods - Implement using Option 1 or Option 2 below ---
            public virtual void LoadSettings() 
            {
                if (File.Exists(_settingsFilePath))
                {
                    try
                    {
                        string jsonString = File.ReadAllText(_settingsFilePath);
                        var savedSettings = JsonSerializer.Deserialize<SettingsData>(jsonString);
                        // ... populate your ViewModel properties from savedSettings ...
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading settings: {ex.Message}");
                        // Handle error or load default settings
                    }
            }
            else
            {
                // Load default settings if the file doesn't exist
            }
        }
            public virtual void SaveSettings() 
            {
            try
            {
                var settingsToSave = new SettingsData
                {
                    // ... populate your SettingsData object from ViewModel properties ...
                };
                string jsonString = JsonSerializer.Serialize(settingsToSave, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsFilePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
            }

        public class SettingsData
        {
            public string PreferredDns { get; set; }
            public string AlternateDns { get; set; }
            public Dictionary<string, string> NetworkDrives { get; set; }
            // ... other properties ...
        }

        // --- Helper Methods ---
        private void UpdateAvailableDriveLetters()
            {
                var initialLetters = GetInitialDriveLetters(); // Get C: to Z:
                var usedLetters = ConfiguredNetworkDrives.Keys;

                var available = initialLetters.Except(usedLetters).OrderBy(l => l);

                // Update the ObservableCollection efficiently
                AvailableDriveLetters.Clear();
                foreach (var letter in available)
                {
                    AvailableDriveLetters.Add(letter);
                }
                // No OnPropertyChanged needed for ObservableCollection content changes if bound correctly
            }

            private List<string> GetInitialDriveLetters()
            {
                // Generates "C:", "D:", ..., "Z:"
                return Enumerable.Range('C', 'Z' - 'C' + 1).Select(i => (char)i + ":").ToList();
                // Optional: You could refine this further by checking DriveInfo.GetDrives()
                // to exclude currently connected physical drives if desired.
            }

        }
    }
