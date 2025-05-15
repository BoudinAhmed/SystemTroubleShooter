using System;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using SystemTroubleShooter.Model;
using System.Management; // Needed for WMI (e.g., getting total RAM)
using System.Diagnostics; // Needed for PerformanceCounter (for CPU/RAM usage - more advanced)


namespace SystemTroubleShooter.ViewModel
{

    public class SystemOverviewViewModel : ViewModelBase // will implementing IDisposable if decide to use PerformanceCounter/Timer
    {

        // --- Backing Fields ---
        private string _machineName;
        private string _osVersion;
        private string _currentUser;
        private string _systemStatus;
        private string _ipAddress;
        private string _version;
        private string _processorName;
        private string _installedRAM;
        private string _cpuUsage;
        private string _ramUsage;
        private ObservableCollection<HistoryEntryModel> _historyEntries;
        private ObservableCollection<string> _selectedIssues = new ObservableCollection<string>();

       
        
        // To add soon for real-time resource usage
        // private PerformanceCounter _cpuCounter;
        // private PerformanceCounter _ramCounter; // Or other memory counters
        // private DispatcherTimer _updateTimer;


        


        // --- Public Properties Binding ---

        // Properties for Top System Info
        public string MachineName
        {
            get => _machineName;
            private set => SetProperty(ref _machineName, value);
        }

        public string OSVersion
        {
            get => _osVersion;
            private set => SetProperty(ref _osVersion, value);
        }

        public string CurrentUser
        {
            get => _currentUser;
            private set => SetProperty(ref _currentUser, value);
        }

        public string SystemStatus
        {
            get => _systemStatus;
            private set => SetProperty(ref _systemStatus, value);
        }

        public string IpAddress
        {
            get => _ipAddress;
            private set => SetProperty(ref _ipAddress, value);
        }

        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }


        // Public Property for History
        public ObservableCollection<HistoryEntryModel> HistoryEntries
        {
            get => _historyEntries;
            set => SetProperty(ref _historyEntries, value);
        }

        // New Public Properties for Specifications and Resources
        public string ProcessorName
        {
            get => _processorName;
            private set => SetProperty(ref _processorName, value);
        }

        public string InstalledRAM
        {
            get => _installedRAM;
            private set => SetProperty(ref _installedRAM, value);
        }

        public string CpuUsage
        {
            get => _cpuUsage;
            private set => SetProperty(ref _cpuUsage, value);
        }

        public string RamUsage
        {
            get => _ramUsage;
            private set => SetProperty(ref _ramUsage, value);
        }


        // --- Constructor ---
        public SystemOverviewViewModel()
        {
           
            HistoryEntries = new ObservableCollection<HistoryEntryModel>();

            // Load data / Set initial values
            LoadSystemInfo(); 
            UpdateSystemStatus();
            LoadHistory();
            LoadVersionInfo();

            // TODO: Implement timer or event listener to call UpdateSystemStatus() and UpdateResourceUsage() periodically
            // If I decide to use PerformanceCounter for resources, initialize counters and timer here
            // InitializeResourceMonitor();

        }

        // For PerformanceCounter/Timer, implement IDisposable sample for future
        /*
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _updateTimer?.Stop();
                    _updateTimer = null;
                    _cpuCounter?.Dispose();
                    _cpuCounter = null;
                    _ramCounter?.Dispose();
                    _ramCounter = null;
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        */
  

        // --- Data Loading Methods ---

        private void LoadSystemInfo()
        {
            // Fetch basic system info
            MachineName = Environment.MachineName;
            OSVersion = Environment.OSVersion.VersionString;
            CurrentUser = Environment.UserName;
            IpAddress = GetLocalIPAddress() ?? "N/A";

            // To load Specifications (basic examples)
            // might need WMI or other APIs.
            ProcessorName = Environment.Is64BitOperatingSystem ? "64-bit Processor Detected" : "32-bit Processor Detected";
            // A better way to get the actual name requires WMI (System.Management)
            // Example: GetProcessorNameWmi();

            InstalledRAM = GetTotalPhysicalMemory() ?? "N/A"; // Use the helper method

            // Load initial Resource Usage (will implement UpdateResourceUsage for live data)
            UpdateResourceUsage();
        }

        // Helper method to get total physical memory using WMI (requires System.Management)
        private string GetTotalPhysicalMemory()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"))
                {
                    foreach (var item in searcher.Get())
                    {
                        var memoryBytes = Convert.ToUInt64(item["TotalPhysicalMemory"]);
                        // Convert bytes to GB
                        var memoryGB = Math.Round((double)memoryBytes / 1024 / 1024 / 1024, 2);
                        return $"{memoryGB} GB";
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., WMI not available, permissions)
                Debug.WriteLine($"Error getting total physical memory via WMI: {ex.Message}");
            }
            return "N/A"; // Fallback value
        }

        // Helper method for Processor Name using WMI with System.Management
        /*
        private string GetProcessorNameWmi()
        {
             try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor"))
                {
                    foreach (var item in searcher.Get())
                    {
                         return item["Name"]?.ToString() ?? "N/A";
                    }
                }
            }
             catch (Exception ex)
            {
                 Debug.WriteLine($"Error getting processor name via WMI: {ex.Message}");
            }
            return "N/A"; // Fallback
        }
        */


        // Method to update resource usage (Will need to implement PerformanceCounter logic for live data)
        private void UpdateResourceUsage()
        {
            // TODO: Implement actual CPU and RAM usage fetching using PerformanceCounter
            // This requires adding PerformanceCounter instances, updating them periodically
            // using a timer (e.g., DispatcherTimer), and disposing them properly.
            // It might also require running the application as administrator
            // depending on system configuration and how PerformanceCounters are set up.

            // --- Placeholder values ---
            // Could show application-specific memory usage relatively easily:
            long currentProcessMemory = Environment.WorkingSet;
            RamUsage = $"{Math.Round((double)currentProcessMemory / 1024 / 1024, 2)} MB (App)"; // Example App Memory

            // Getting total system CPU/RAM usage percentage is more complex without PerformanceCounter
            CpuUsage = "-- %"; // Placeholder
            // RamUsage = "-- %"; // Placeholder for system-wide RAM percentage

            // If using PerformanceCounter and Timer:
            /*
            try
            {
                if(_cpuCounter != null) CpuUsage = $"{Math.Round(_cpuCounter.NextValue(), 1)}%";
                // If using Memory\Available MBytes:
                // if(_ramCounter != null) RamUsage = $"{Math.Round(_ramCounter.NextValue(), 1)} MB Available";
                // If calculating percentage of total RAM:
                // if (_ramCounter != null && InstalledRAM != "N/A") { ... calculate percentage ... }

            }
            catch (Exception ex)
            {
                 Debug.WriteLine($"Error updating resource usage: {ex.Message}");
                 CpuUsage = "Error";
                 RamUsage = "Error";
            }
            */
        }

        // Maybe: Method to initialize Performance Counters and Timer
        /*
        private void InitializeResourceMonitor()
        {
             try
             {
                 _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                 // Choose appropriate memory counter:
                 // "Memory", "Available MBytes"
                 // "Memory", "% Committed Bytes In Use"
                 _ramCounter = new PerformanceCounter("Memory", "Available MBytes");


                 _updateTimer = new DispatcherTimer();
                 _updateTimer.Interval = TimeSpan.FromSeconds(1); // Update frequency
                 _updateTimer.Tick += (s, e) => UpdateResourceUsage(); // Call update method on tick

                 // Read initial values (first read is often 0)
                 _cpuCounter.NextValue();
                 _ramCounter.NextValue();

                 _updateTimer.Start();
             }
             catch (Exception ex)
             {
                 // Handle exceptions if Performance Counters are not available or permissions are denied
                 Debug.WriteLine($"Failed to initialize performance counters: {ex.Message}");
                 CpuUsage = "N/A (Perf Counter Error)";
                 RamUsage = "N/A (Perf Counter Error)";
             }
        }
        */


        private void UpdateSystemStatus()
        {
            // Determine network status
            bool isOnline = NetworkInterface.GetIsNetworkAvailable();
            SystemStatus = isOnline ? "Online" : "Offline";
            // if consider adding other system health checks here
        }

        

        private void LoadHistory()
        {
            // Will pull from cache file eventually
            HistoryEntries.Add(new HistoryEntryModel { Timestamp = DateTime.Now.AddHours(-1), IssueDescription = "Internet issue not access browser", ResolutionStatus = "Fixed" });
            HistoryEntries.Add(new HistoryEntryModel { Timestamp = DateTime.Now.AddDays(-1), IssueDescription = "Sound driver needed reinstall", ResolutionStatus = "Fixed" });
            HistoryEntries.Add(new HistoryEntryModel { Timestamp = DateTime.Now.AddDays(-2), IssueDescription = "Windows Update stuck", ResolutionStatus = "Pending" });
            // Add more dummy data for testing appearance if needed
            HistoryEntries.Add(new HistoryEntryModel { Timestamp = DateTime.Now.AddHours(-5), IssueDescription = "Application X crashing", ResolutionStatus = "Fixed" });
            HistoryEntries.Add(new HistoryEntryModel { Timestamp = DateTime.Now.AddDays(-3), IssueDescription = "High CPU usage detected", ResolutionStatus = "Investigating" });
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
    }
}