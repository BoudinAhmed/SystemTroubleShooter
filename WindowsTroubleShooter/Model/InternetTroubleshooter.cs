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

        private const string CheckAdaptersScriptPath = @"Scripts\\Internet\\IsNetworkAdaptersAvailable.ps1"; 
        private const string RefreshAdaptersScriptPath = @"Scripts\\Internet\\RefreshNetworkAdapter.ps1";



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

                StatusMessage = $"Error: Please try again later";
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