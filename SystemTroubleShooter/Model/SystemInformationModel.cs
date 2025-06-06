using System;
using System.Diagnostics;
using System.IO;

namespace SystemTroubleShooter.Model
{
    public class SystemInformationModel
    {
        public int RamUsagePercentage { get; set; }
        public double CpuUsagePercentage { get; set; }
        public long FreeDiskSpaceGB { get; set; }
        public long DiskSpaceTotalGB { get; set; }

        public SystemInformationModel()
        {
            
            RamUsagePercentage = 0;
            CpuUsagePercentage = 0.0;
            FreeDiskSpaceGB = 0;
            DiskSpaceTotalGB = 0;
        }

        public SystemInformationModel GetSystemStats(string relativeScriptPath)
        {
            var systemInfoResult = new SystemInformationModel();

            try
            {
                // To construct the absolute script path
                string executionDirectory = AppDomain.CurrentDomain.BaseDirectory;
                // Using Path.Combine and Path.GetFullPath handles forward/backward slashes
                string scriptPath = Path.GetFullPath(Path.Combine(executionDirectory, relativeScriptPath));

                if (!File.Exists(scriptPath))
                {
                    Debug.WriteLine($"Error: PowerShell script not found at: {scriptPath}");
                    // Set default/error values in the result object if script not found
                    return systemInfoResult;
                }

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true, // Redirect errors to capture script failures
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    string errorOutput = process.StandardError.ReadToEnd(); // Read error stream
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        // Log PowerShell script errors
                        Debug.WriteLine($"PowerShell script exited with code {process.ExitCode}.");
                        Debug.WriteLine($"PowerShell Error Output: {errorOutput}");
                        // No need to throw, just return the default initialized object
                        return systemInfoResult;
                    }
                    else
                    {
                        // Manually parse the delimited string output into the result object
                        ParseSystemStatsOutput(output, systemInfoResult);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception when executing PowerShell script: {ex.Message}");
                // No need to throw, return the default initialized object
            }

            return systemInfoResult; // Return the populated (or default) object
        }

        private void ParseSystemStatsOutput(string output, SystemInformationModel target)
        {
            if (string.IsNullOrWhiteSpace(output))
            {
                Debug.WriteLine("PowerShell script returned empty output.");
                // target already initialized to defaults
                return;
            }

            try
            {
                // Expected output format: "RamUsagePercentage,CpuUsagePercentage,FreeDiskSpaceGB"
                string[] parts = output.Trim().Split(',');

                if (parts.Length == 3)
                {
                    // Use TryParse for safer conversion
                    if (int.TryParse(parts[0], out int ramUsage))
                        target.RamUsagePercentage = ramUsage;
                    else Debug.WriteLine($"Failed to parse RamUsagePercentage: {parts[0]}");

                    if (double.TryParse(parts[1], out double cpuUsage))
                        target.CpuUsagePercentage = cpuUsage;
                    else Debug.WriteLine($"Failed to parse CpuUsagePercentage: {parts[1]}");

                    if (long.TryParse(parts[2], out long freeDisk))
                        target.FreeDiskSpaceGB = freeDisk;
                    else Debug.WriteLine($"Failed to parse FreeDiskSpaceGB: {parts[2]}");

                }
                else
                {
                    Debug.WriteLine($"Unexpected PowerShell output format. Expected 4 parts, got {parts.Length}. Output: '{output}'");
                    // target remains initialized to defaults
                }
            }
            catch (Exception ex) // Catch all parsing exceptions
            {
                Debug.WriteLine($"Error during parsing of PowerShell output: {ex.Message}");
                Debug.WriteLine($"Raw PowerShell output that caused error: '{output}'");
                // target remains initialized to defaults
            }
        }
    }
}