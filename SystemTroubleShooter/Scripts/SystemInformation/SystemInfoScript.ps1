# SystemInfoScript.ps1

# Ensure error output is redirected for debugging purposes in C#
# This will write any PowerShell errors to Standard Error, which C# can capture.
$ErrorActionPreference = "Continue" # Continue will show errors, but not stop execution

# Initialize variables to default values to prevent null/empty output if a part fails
$ramUsagePercentage = 0.0 # Changed to double to match typical percentage calculations
$cpuUsage = 0.0
$usedDiskSpaceGB = 0

# --- CPU Usage ---
$cpuCounter = $null
try {
    # Add -ErrorAction SilentlyContinue to prevent script from stopping on a counter issue
    $cpuCounter = New-Object System.Diagnostics.PerformanceCounter("Processor", "% Processor Time", "_Total", $true)
    $cpuCounter.NextValue() | Out-Null # Call NextValue once to initialize
    Start-Sleep -Milliseconds 100 # Wait a bit for accurate reading
    $cpuReading = $cpuCounter.NextValue()

    # Validate CPU reading
    if ([double]::IsInfinity($cpuReading) -or [double]::IsNaN($cpuReading)) {
        Write-Error "CPU usage reading resulted in Infinity or NaN. Setting to 0.0."
        $cpuUsage = 0.0
    } else {
        $cpuUsage = [math]::Round($cpuReading, 2)
    }

} catch {
    Write-Error "Failed to get CPU usage: $_"
    $cpuUsage = 0.0 # Set to default on error
} finally {
    if ($cpuCounter) { $cpuCounter.Dispose() } # Clean up the performance counter
}

# --- RAM Usage ---
try {
    # Changed from Get-CimInstance CIM_OperatingSystem to Get-WmiObject Win32_ComputerSystem
    $computerInfo = Get-WmiObject Win32_ComputerSystem -ErrorAction SilentlyContinue
    if ($computerInfo) {
        $totalRamBytes = $computerInfo.TotalPhysicalMemory # This property is directly available
        # FreePhysicalMemory is not directly available on Win32_ComputerSystem.
        # We need Win32_OperatingSystem for that, or calculate differently.

        # Option 1: Use Win32_OperatingSystem for free memory (more accurate percentage)
        $osInfo = Get-WmiObject Win32_OperatingSystem -ErrorAction SilentlyContinue
        if ($osInfo) {
            $freeRamBytes = $osInfo.FreePhysicalMemory * 1KB # Convert from KB to Bytes
            $usedRamBytes = $totalRamBytes - $freeRamBytes
            if ($totalRamBytes -gt 0) { # Prevent division by zero
                $ramReading = ($usedRamBytes / $totalRamBytes) * 100
                if ([double]::IsInfinity($ramReading) -or [double]::IsNaN($ramReading)) {
                    Write-Error "RAM usage reading resulted in Infinity or NaN. Setting to 0.0."
                    $ramUsagePercentage = 0.0
                } else {
                    $ramUsagePercentage = [math]::Round($ramReading, 2)
                }
            } else {
                Write-Error "Total Physical Memory is 0 (from Win32_ComputerSystem). Setting RAM usage to 0.0."
                $ramUsagePercentage = 0.0
            }
        } else {
            Write-Error "Failed to get Win32_OperatingSystem instance for free memory. Setting RAM usage to 0.0."
            $ramUsagePercentage = 0.0
        }

        # Option 2 (Simpler, but less accurate for percentage - just gives available RAM in MB)
        # You could just get "Available MBytes" from PerformanceCounter "Memory" if you want a simpler RAM usage.
        # This script is geared towards percentage, so Option 1 is better for consistency.
    } else {
        Write-Error "Failed to get Win32_ComputerSystem instance. Setting RAM usage to 0.0."
        $ramUsagePercentage = 0.0
    }
} catch {
    Write-Error "Failed to get RAM usage: $_"
    $ramUsagePercentage = 0.0 # Set to default on error
}

# --- Disk Space for C: Drive ---
try {
    $driveInfo = Get-PSDrive C -ErrorAction SilentlyContinue

    if ($driveInfo) {
        $usedDiskSpaceGB = [math]::Round($driveInfo.Used / 1GB, 0)
    } else {
        Write-Error "Failed to get C: drive information. Setting disk space to 0."
        $usedDiskSpaceGB = 0
    }
} catch {
    Write-Error "Failed to get disk space: $_"
    $usedDiskSpaceGB = 0
}

# Output the values as a comma-separated string to StandardOutput
# Ensure the order matches the C# parsing logic:
# RamUsagePercentage,CpuUsagePercentage,DiskSpaceUsedGB,DiskSpaceTotalGB

Write-Output ("{0},{1},{2}" -f `
    [int]$ramUsagePercentage, ` # Cast to int here to match C# property type
    [double]$cpuUsage, `      # Keep as double for C# property type
    [long]$usedDiskSpaceGB) # Cast to long here to match C# property type
   