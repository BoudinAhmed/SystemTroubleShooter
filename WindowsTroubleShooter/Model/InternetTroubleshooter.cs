using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Threading; // Add this using directive

namespace WindowsTroubleShooter.Model
{
    public class InternetTroubleshooter : BaseTroubleshooter
    {
        private const string PingCommand = "ping google.ca";
        private const string WifiInterfaceName = "Wi-Fi";
        private const string EthernetInterfaceName = "Ethernet 4";

        public InternetTroubleshooter()
        {
            IssueType = "Internet Connection";
            Detail = "Fix problems with connecting to the internet";
            TimeStamp = DateTime.Now;
            TaskList = new List<string>
            {
                "Checking Internet Connection",
                "Refreshing Network Adapter",
                "Network Reset"
            };
        }

        public bool IsConnected()
        {
            return ExecuteCommand(PingCommand);
        }

        public void RefreshNetworkAdapter()
        {
            try
            {
                ExecuteCommand($"netsh interface set interface \"{WifiInterfaceName}\" disable");
                ExecuteCommand($"netsh interface set interface \"{EthernetInterfaceName}\" disable");

                ExecuteCommand($"netsh interface set interface \"{WifiInterfaceName}\" enable");
                ExecuteCommand($"netsh interface set interface \"{EthernetInterfaceName}\" enable");

                if (IsConnected())
                {
                    ResolutionMessage = "Fix: Refresh Network Adapter";
                    IsFixed = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing network adapter: {ex.Message}");
            }
        }

        public void NetworkReset()
        {
            if (IsConnected())
            {
                ResolutionMessage = "Fix: Network Reset";
                IsFixed = true;
            }
        }

        public override async Task<string> RunDiagnosticsAsync()
        {
            // Use Dispatcher.InvokeAsync to update StatusMessage on the UI thread
            await Dispatcher.CurrentDispatcher.InvokeAsync(() => StatusMessage = "Troubleshooting Internet");
            await Task.Delay(2000);

            await Dispatcher.CurrentDispatcher.InvokeAsync(() => StatusMessage = "Refreshing Network Adapter");
            await Task.Delay(2000);
            RefreshNetworkAdapter();

            if (IsFixed)
            {
                await Dispatcher.CurrentDispatcher.InvokeAsync(() => StatusMessage = "Internet Connection Fixed");
                await Task.Delay(2000);
                return "Internet Connection Fixed";
            }

            await Task.Delay(2000);
            await Dispatcher.CurrentDispatcher.InvokeAsync(() => StatusMessage = "Network Reset");
            await Task.Delay(2000);
            NetworkReset();

            return "Internet Connection failed";
        }
    }
}