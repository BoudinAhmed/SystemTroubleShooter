using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WindowsTroubleShooter.Helpers.Commands;


namespace WindowsTroubleShooter.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        // Inherit from *your* ViewModelBase
       
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

            // Using ObservableCollection for AvailableDriveLetters as it's common for ComboBoxes
            // and handles UI updates automatically when items are added/removed.
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
                // --- Initialize based on which option you choose ---
                // If using Option 2 (JSON File):
                // string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                // string appFolder = Path.Combine(appDataFolder, "WindowsTroubleShooter"); // Use your app name
                // Directory.CreateDirectory(appFolder);
                // _networkDrivesSettingsFilePath = Path.Combine(appFolder, "network_drives.json");

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
            public virtual void LoadSettings() { /* Implementation specific */ }
            public virtual void SaveSettings() { /* Implementation specific */ }

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
