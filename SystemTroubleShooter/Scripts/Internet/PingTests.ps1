# Define the initial targets for ping tests
$targets = @("127.0.0.1", "8.8.8.8", "google.com")
$failedTargets = @() # Array to store targets that fail the ping test

# Dynamically get the default gateway and add it to the targets list
try {
    $defaultGateway = (Get-NetIPConfiguration | Where-Object { $_.IPv4DefaultGateway -ne $null }).IPv4DefaultGateway.NextHop
    if ($defaultGateway) {
        $targets += $defaultGateway
    } else {
        # This is an informational message, not a status update for the main UI
        Write-Output "INFO: No default gateway found to test."
    }
} catch {
    Write-Output "INFO: Could not determine default gateway."
}


# Perform ping tests for each target
foreach ($target in $targets) {
    # This is an intermediate status update
    Write-Output "Pinging $target..."
    
    # Run Test-Connection and check for a valid response
    $pingResult = Test-Connection -ComputerName $target -Count 1 -ErrorAction SilentlyContinue

    if ($pingResult) {
        # Intermediate success update
        Write-Output "Ping to $target successful."
    } else {
        # Intermediate failure update
        Write-Output "Ping to $target failed."
        $failedTargets += $target # Add the failed target to our list
    }
}

# --- Final Result ---
# Based on the test outcomes, output a single, specific "FINAL:" message for the pop-up.

if ($failedTargets.Count -eq 0) {
    # SUCCESS: All pings were successful.
    Write-Output "FINAL:Network connectivity tests completed successfully. All targets, including local, internet, and gateway, responded as expected."
    exit 0
} else {
    # FAILURE: Some pings failed.
    $failedList = $failedTargets -join ", " # Create a comma-separated string of failed targets
    Write-Output "FINAL:Network connectivity test failed. Could not reach the following targets: $failedList. This may indicate a problem with your local network or internet connection."
    exit 1
}