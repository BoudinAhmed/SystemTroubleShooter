# Refresh-IpLease.ps1
# Releases and renews the IP address lease for the specified adapter.

param(
    [Parameter(Mandatory=$true)]
    [string]$AdapterName
)

# Use Write-Output for messages intended for the C# UI
Write-Output "Resfreshing IP lease..."

# --- Configuration ---
# ... (config variables) ...

# --- Release IP Address ---
Write-Output "Releasing IP address for adapter '$AdapterName'" # Changed Write-Host
try {
    Release-NetIPAddress -InterfaceAlias $AdapterName -ErrorAction Stop
    Write-Output "INFO: IP address release command sent successfully." # Changed Write-Host
    Start-Sleep -Seconds $WaitAfterReleaseSeconds

} catch {
    Write-Error "ERROR: Error releasing IP address for adapter '$AdapterName': $($_.Exception.Message)" # Keep Write-Error
    exit 1
}

# --- Renew IP Address ---
Write-Output "STATUS: Renewing IP address for adapter '$AdapterName'..." # Changed Write-Host
try {
    Renew-NetIPAddress -InterfaceAlias $AdapterName -ErrorAction Stop
    Write-Output "INFO: IP address renew command sent successfully." # Changed Write-Host
    Start-Sleep -Seconds $WaitAfterRenewInitialSeconds

} catch {
    Write-Error "ERROR: Error renewing IP address for adapter '$AdapterName': $($_.Exception.Message)" # Keep Write-Error
    exit 1
}

# --- Wait for Valid IP Address ---
Write-Output "STATUS: Waiting for a valid IP address to be assigned (timeout: $($IpAssignTimeoutSeconds)s)..." # Changed Write-Host

# ... (Waiting loop logic - keep Write-Host inside loop for intermediate messages if desired, or change to Write-Output)
# For messages inside the loop you want the C# app to see:
# Write-Output "INFO: Still waiting for IP address..." # Changed Write-Host in loop

# --- Final Outcome ---
Write-Output "STATUS: IP lease refresh complete and valid IP address '$assignedIp' assigned." # Changed Write-Host
exit 0