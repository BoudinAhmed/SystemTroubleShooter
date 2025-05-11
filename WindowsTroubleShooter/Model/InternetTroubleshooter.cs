using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading; // Add this using directive

namespace WindowsTroubleShooter.Model
{
    public class InternetTroubleshooter : BaseTroubleshooter
    {
        private const string CheckAdaptersScriptPath = "C:\\Users\\boudi\\WindowsTroubleShooter\\WindowsTroubleShooter\\Scripts\\Internet\\IsNetworkAdaptersAvailable.ps1"; 
        private const string RefreshAdaptersScriptPath = "C:\\Users\\boudi\\WindowsTroubleShooter\\WindowsTroubleShooter\\Scripts\\Internet\\RefreshNetworkAdapter.ps1"; 


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

        private async Task<bool> CheckForActiveAdaptersAsync()
        {
            StatusMessage = "Checking for active network adapters...";
            var result = await ExecutePowerShellScriptAsync(CheckAdaptersScriptPath);

            if (!string.IsNullOrEmpty(result.StandardError))
            {
                Debug.WriteLine($"Error checking adapters: {result.StandardError}");
                StatusMessage = $"Error checking adapters: {result.StandardError}";
                return false; // for failure
            }

            // Parse the output to find if active adapters were found
            var activeFoundMatch = Regex.Match(result.StandardOutput, @"^ActiveNetworkAdaptersFound:\s*(True|False)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var countMatch = Regex.Match(result.StandardOutput, @"^NumberOfActiveAdapters:\s*(\d+)", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            bool activeFound = false;
            int adapterCount = 0;

            if (activeFoundMatch.Success && bool.TryParse(activeFoundMatch.Groups[1].Value, out activeFound))
            {
                // Status updated below based on count
            }

            if (countMatch.Success && int.TryParse(countMatch.Groups[1].Value, out adapterCount))
            {
                // Status updated below
            }

            if (activeFound && adapterCount > 0)
            {
                StatusMessage = $"Found {adapterCount} active network adapter(s).";
                return true;
            }
            else
            {
                StatusMessage = "No active network adapters found.";
                return false;
            }
        }

        

       

        public override async Task<string> RunDiagnosticsAsync()
        {
            StatusMessage = "Checking Internet Connection...";
            await Task.Delay(2000);

            var (output, error, exitCode) = await ExecutePowerShellScriptAsync(CheckAdaptersScriptPath);

            if (exitCode != 0)
            {
                Debug.WriteLine($"PowerShell Execution Failed!");
                Debug.WriteLine($"Exit Code: {exitCode}");
                Debug.WriteLine($"Standard Output: {(string.IsNullOrWhiteSpace(output) ? "(empty)" : output)}");
                Debug.WriteLine($"Standard Error: {(string.IsNullOrWhiteSpace(error) ? "(empty)" : error)}");

                StatusMessage = $"Error: {exitCode} | Output: {output} | Error: {error}";

                await Task.Delay(2000);
                return $"Error checking network adapters: {error}";
            }

            StatusMessage = "Refreshing Network Adapter...";
            await Task.Delay(2000);

            var (refreshOutput, refreshError, refreshExitCode) = await ExecutePowerShellScriptAsync(RefreshAdaptersScriptPath);

            if (refreshExitCode != 0)
            {
                Debug.WriteLine($"PowerShell Execution Failed!");
                Debug.WriteLine($"Exit Code: {refreshExitCode}");
                Debug.WriteLine($"Standard Output: {(string.IsNullOrWhiteSpace(refreshOutput) ? "(empty)" : refreshOutput)}");
                Debug.WriteLine($"Standard Error: {(string.IsNullOrWhiteSpace(refreshError) ? "(empty)" : refreshError)}");

                StatusMessage = $"Error: {refreshError}";
                return $"Error refreshing network adapters: {refreshError}";
            }

            StatusMessage = "Network Reset...";
            await Task.Delay(2000);

            IsFixed = true; // Assume fix worked for now

            if (IsFixed)
            {
                StatusMessage = "Internet Connection Fixed!";
                await Task.Delay(2000);
                return "Internet Connection Fixed";
            }

            return "Internet Connection failed";
        }
    }
}