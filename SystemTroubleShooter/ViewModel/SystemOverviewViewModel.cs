﻿using System;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using SystemTroubleShooter.Model;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Threading;


namespace SystemTroubleShooter.ViewModel
{

    public class SystemOverviewViewModel : ViewModelBase, IDisposable
    {

        // --- Backing Fields ---
        private string? _machineName;
        private string? _osVersion;
        private string? _currentUser;
        private string? _systemStatus;
        private string? _ipAddress;
        private string? _version;
        private string? _processorName;

        private readonly string _systemInformationScript = @"Scripts\SystemInformation\SystemInfoScript.ps1";
        private SystemInformationModel? _currentSystemInfo;
        private ObservableCollection<HistoryEntryModel>? _historyEntries;
        private readonly DispatcherTimer _updateTimer;




        // --- Public Properties Binding ---

        // Properties for Top System Info
        public string? MachineName
        {
            get => _machineName;
            private set => SetProperty(ref _machineName, value);
        }

        public string? OSVersion
        {
            get => _osVersion;
            private set => SetProperty(ref _osVersion, value);
        }

        public string? CurrentUser
        {
            get => _currentUser;
            private set => SetProperty(ref _currentUser, value);
        }

        public string? SystemStatus
        {
            get => _systemStatus;
            private set => SetProperty(ref _systemStatus, value);
        }

        public string? IpAddress
        {
            get => _ipAddress;
            private set => SetProperty(ref _ipAddress, value);
        }

        public string? Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }

        // Public Property for Current System Information
        public SystemInformationModel? CurrentSystemInfo
        {
            get => _currentSystemInfo;
            set
            {
                // Only update and notify if the new value is different
                if (!Equals(_currentSystemInfo, value))
                {
                    if(value is not null)
                    _currentSystemInfo = value;
                    OnPropertyChanged();
                    // Also notify that CPUUsage and RamUsage (now derived from CurrentSystemInfo) have changed
                    OnPropertyChanged(nameof(CpuUsageDisplay));
                    OnPropertyChanged(nameof(RamUsageDisplay));
                    OnPropertyChanged(nameof(FreeDiskSpaceDisplay));
                }
            }
        }

        // Public Property for History
        public ObservableCollection<HistoryEntryModel>? HistoryEntries
        {
            get => _historyEntries;
            set => SetProperty(ref _historyEntries, value);
        }

        // New Public Properties for Specifications and Resources
        public string? ProcessorName
        {
            get => _processorName;
            private set => SetProperty(ref _processorName, value);
        }

        

        // Display properties for CPU, RAM, Disk Space derived from CurrentSystemInfo
        // These provide formatted strings for UI binding
        public string CpuUsageDisplay => CurrentSystemInfo != null ? $"{CurrentSystemInfo.CpuUsagePercentage:F1}%" : "-- %";
        public string RamUsageDisplay => CurrentSystemInfo != null ? $"{CurrentSystemInfo.RamUsagePercentage:F1}%" : "-- %";
        public string FreeDiskSpaceDisplay => CurrentSystemInfo != null ?
            $"{CurrentSystemInfo.FreeDiskSpaceGB-CurrentSystemInfo.DiskSpaceTotalGB} GB" : "-- GB";


        // --- Constructor ---
        public SystemOverviewViewModel()
        {   

            HistoryEntries = new ObservableCollection<HistoryEntryModel>();

            // Load data / Set initial values
            LoadSystemInfo(); 
            LoadHistory();
            LoadVersionInfo();

            // Timer to call UpdateSystemData() 
            _updateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };

            _updateTimer.Tick += UpdateSystemDataAsync;

            _updateTimer.Start();
            UpdateSystemDataAsync(null, EventArgs.Empty);

        }

        
  

        // --- Data Loading Methods ---

        private async void LoadSystemInfo()
        {
            //For getting System Resources
            CurrentSystemInfo = new SystemInformationModel();

            // Fetch basic system info
            MachineName = Environment.MachineName;
            OSVersion = Environment.OSVersion.VersionString;
            CurrentUser = Environment.UserName;
            IpAddress = GetLocalIPAddress() ?? "N/A";

            ProcessorName = Environment.Is64BitOperatingSystem ? "64-bit Processor Detected" : "32-bit Processor Detected";
            

            // Load PowerShell data asynchronously
            await LoadResourceUsageAsync();
        }

        private async Task LoadResourceUsageAsync()
        {
            // Run the PowerShell script on a background thread to prevent UI freezing
            await Task.Run(() =>
            {
                SystemInformationModel systemInfoFetcher = new SystemInformationModel();
                // Update CurrentSystemInfo property, which will trigger UI update
                if (_systemInformationScript is not null)
                CurrentSystemInfo = systemInfoFetcher.GetSystemStats(_systemInformationScript);
            });

            
        }
        

        private async void UpdateSystemDataAsync(object? sender, EventArgs e)
        {
            // --- Update System Status ---
            SystemStatus = NetworkInterface.GetIsNetworkAvailable() ? "Online" : "Offline";

            // --- Update Resources using script on a background thread ---
            try
            {
                var systemInfoFetcher = new SystemInformationModel();

                // Pushes the script execution to a background thread
                var newInfo = await Task.Run(() => systemInfoFetcher.GetSystemStats(_systemInformationScript));

                // Once await completes, we are back on the UI thread and can safely update properties.
                _currentSystemInfo = newInfo;

                // Notify the UI the changes
                OnPropertyChanged(nameof(CpuUsageDisplay));
                OnPropertyChanged(nameof(RamUsageDisplay));
                OnPropertyChanged(nameof(FreeDiskSpaceDisplay));
            }
            catch (Exception ex)
            {
                // For potential errors from the script
                Debug.WriteLine($"Error updating system data from script: {ex.Message}");
            }
        }


        private void LoadHistory()
        {
            // Will pull from cache file eventually
            HistoryEntries?.Add(new HistoryEntryModel { Timestamp = DateTime.Now.AddHours(-1), IssueDescription = "Internet issue not access browser", ResolutionStatus = "Fixed" });
            HistoryEntries?.Add(new HistoryEntryModel { Timestamp = DateTime.Now.AddDays(-1), IssueDescription = "Sound driver needed reinstall", ResolutionStatus = "Fixed" });
            HistoryEntries?.Add(new HistoryEntryModel { Timestamp = DateTime.Now.AddDays(-2), IssueDescription = "Windows Update stuck", ResolutionStatus = "Pending" });
            // Add more dummy data for testing appearance if needed
            HistoryEntries?.Add(new HistoryEntryModel { Timestamp = DateTime.Now.AddHours(-5), IssueDescription = "Application X crashing", ResolutionStatus = "Fixed" });
            HistoryEntries?.Add(new HistoryEntryModel { Timestamp = DateTime.Now.AddDays(-3), IssueDescription = "High CPU usage detected", ResolutionStatus = "Investigating" });
        }

        private void LoadVersionInfo()
        {
            // Hardcoded for now but will load it from assembly info
            Version = "Version 2.0.1";
        }

        // --- Helper Methods ---
        private string GetLocalIPAddress()
        {
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) // Get IPv4
                    {
                        return ip.ToString();
                    }
                }
                return "IPv4 Not Found";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving IP address: {ex.Message}");
                return "Error Retrieving IP";
            }
        }
        public void Dispose()
        {
            // Stop timer when view model is disposed
            _updateTimer.Stop();
        }
    }
}