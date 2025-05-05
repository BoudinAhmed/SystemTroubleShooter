using System;
using System.Collections.ObjectModel;
using System.ComponentModel; 
using System.Runtime.CompilerServices; 
using System.Net.NetworkInformation; 
using WindowsTroubleShooter.Interfaces;
using WindowsTroubleShooter.Model;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using WindowsTroubleShooter.Helpers.Commands;

namespace WindowsTroubleShooter.ViewModel
{

    public class DashboardViewModel : ViewModelBase 
    {

        // --- Backing Fields ---
        private string _machineName;
        private string _osVersion;
        private string _currentUser;
        private string _systemStatus;
        private string _ipAddress;
        private string _version;
        private ObservableCollection<IssueItemViewModel> _issueItems;
        private ObservableCollection<HistoryEntryModel> _historyEntries; 
        private ObservableCollection<string> _selectedIssues = new ObservableCollection<string>();

        public ICommand SwitchToProblemListCommand { get; set; }


        // --- Public Properties Binding ---

        // Properties for System Overview
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

        // Properties for Issues and History
        public ObservableCollection<IssueItemViewModel> IssueItems
        {
            get => _issueItems;
            set => SetProperty(ref _issueItems, value);
        }

        public ObservableCollection<HistoryEntryModel> HistoryEntries
        {
            get => _historyEntries;
            set => SetProperty(ref _historyEntries, value);
        }


        // --- Constructor ---
        public DashboardViewModel()
        {
            // Initializing collections
            IssueItems = new ObservableCollection<IssueItemViewModel>();
            HistoryEntries = new ObservableCollection<HistoryEntryModel>(); 

            // Load data / Set initial values
            LoadSystemInfo(); 
            UpdateSystemStatus(); 
            LoadIssueItems();
            LoadHistory();
            LoadVersionInfo();

            // TODO: Implenment timer or event listener call UpdateSystemStatus() in a span time

            // Navigation commands
            SwitchToProblemListCommand = new RelayCommand(SwitchToProblemList);

        }

        public event EventHandler RequestNavigateToProblemList;
        private void SwitchToProblemList(object obj)
        {
            RequestNavigateToProblemList?.Invoke(this, EventArgs.Empty);
        }

        // --- Data Loading Methods ---

        private void LoadSystemInfo()
        {
            // Fetch actual system info
            MachineName = Environment.MachineName;
            OSVersion = Environment.OSVersion.VersionString;
            CurrentUser = Environment.UserName;
            IpAddress = GetLocalIPAddress() ?? "N/A";
        }

        private void LoadVersionInfo()
        {
            // Hardcoded for now but will load it from assembly info
            Version = "Version 2.0.1";
        }

        private void UpdateSystemStatus()
        {
            //Determine network status
            bool isOnline = NetworkInterface.GetIsNetworkAvailable();
            

            SystemStatus = isOnline ? "Online" : "Offline"; // todo update the property
        }

        private void LoadIssueItems()
        {
            
            // Add items
            IssueItems.Add(new IssueItemViewModel { Title = "Internet Connection", Description = "Fix problems with connecting to the internet", ImageSource = "\xE701", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Windows Update", Description = "Resolve problem with windows update", ImageSource = "\xE895", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Sound", Description = "Fix problems with playing audio", ImageSource = "\xE767", IssueType = new InternetTroubleshooter() });
            IssueItems.Add(new IssueItemViewModel { Title = "Map Network Drive", Description = "Map a network drive with letter and path", ImageSource = "\xE8CE", IssueType = new InternetTroubleshooter() });
            // ... will pull from cache file eventually
        }

        private void LoadHistory()
        {
            // Will pull from cache file eventually
            HistoryEntries.Add(new HistoryEntryModel { Timestamp = DateTime.Now.AddHours(-1), IssueDescription = "Internet issue not access browser", ResolutionStatus = "Fixed" });
            HistoryEntries.Add(new HistoryEntryModel { Timestamp = DateTime.Now.AddDays(-1), IssueDescription = "Sound driver needed reinstall", ResolutionStatus = "Fixed" });
            HistoryEntries.Add(new HistoryEntryModel { Timestamp = DateTime.Now.AddDays(-2), IssueDescription = "Windows Update stuck", ResolutionStatus = "Pending Investigation" });
        }

        // --- Helper Methods ---
        private string GetLocalIPAddress()
        {
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) // Get that ip
                    {
                        return ip.ToString();
                    }
                }
                return "IPv4 Not found";
            }
            catch (Exception)
            {
                return "Error retrieving IP";
            }
        }

        
    }
}