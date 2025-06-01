

# Defining the Bluetooth services to check
$bluetoothServices = @(
    @{ Name = "BthServ"; DisplayName = "Bluetooth Support Service" },
    @{ Name = "bthhfenum"; DisplayName = "Bluetooth Audio Gateway Service" },
    @{ Name = "HidBth"; DisplayName = "Bluetooth User Support Service" } )

ForEach ($serviceInfo in $bluetoothServices) {
    $serviceName = $serviceInfo.Name
    $serviceDisplayName = $serviceInfo.DisplayName
    Write-Host "`nChecking service: $($serviceDisplayName) ($($serviceName))"

    try {
        $service = Get-Service -Name $serviceName -ErrorAction Stop

        # Checking and setting Startup Type to Automatic
        If ($service.StartType -ne "Automatic") {
            Write-Host "  Service startup type is $($service.StartType). Setting to Automatic..."
            try {
                Set-Service -Name $serviceName -StartupType Automatic -ErrorAction Stop
                Write-Host "  Successfully set startup type to Automatic." -ForegroundColor Green
            }
            catch {
                Write-Warning "  Failed to set startup type for $($serviceName). Error: $($_.Exception.Message)"
                Write-Warning "  Please ensure you are running PowerShell as an Administrator."
            }
        } else {
            Write-Host "  Service startup type is already Automatic."
        }

        # Checking if the service is running and start it if it's stopped
        If ($service.Status -ne "Running") {
            Write-Host "  Service is currently $($service.Status). Attempting to start..."
            try {
                Start-Service -Name $serviceName -ErrorAction Stop
                # Wait for a moment and check status again
                Start-Sleep -Seconds 2
                $service = Get-Service -Name $serviceName # Refresh service object
                If ($service.Status -eq "Running") {
                    Write-Host "  Successfully started $($serviceName)." -ForegroundColor Green
                } else {
                    Write-Warning "  Failed to start $($serviceName). Current status: $($service.Status)."
                }
            }
            catch {
                Write-Warning "  Error starting service $($serviceName). Error: $($_.Exception.Message)"
                Write-Warning "  Please ensure you are running PowerShell as an Administrator."
            }
        } else {
            Write-Host "  Service is already running." -ForegroundColor Green
        }
    }
    catch [Microsoft.PowerShell.Commands.ServiceCommandException] {
        Write-Warning "  Service $($serviceDisplayName) ($($serviceName)) not found or error accessing it. It might not be installed on this system."
    }
    catch {
        Write-Warning "  An unexpected error occurred while checking service $($serviceName): $($_.Exception.Message)"
    }
}

Write-Host "`n--- Bluetooth Service Check Complete ---" -ForegroundColor Yellow

# Note about HidBth and BluetoothUserService:
Write-Host "`nImportant Note on 'HidBth':" -ForegroundColor Cyan
Write-Host "The PnP device class 'HidBth' is crucial for Bluetooth keyboards, mice, etc." -ForegroundColor Cyan
Write-Host "The service often associated with user-mode Bluetooth HID interactions is 'Bluetooth User Support Service'." -ForegroundColor Cyan
Write-Host "Its actual service name can vary (e.g., 'BluetoothUserService' or 'BluetoothUserService_xxxxx')." -ForegroundColor Cyan
Write-Host "The script attempts to manage 'BthServ' and 'bthhfenum' which are foundational." -ForegroundColor Cyan
Write-Host "If issues persist with HID devices, ensure drivers are correct and check for a 'Bluetooth User Support Service' in services.msc." -ForegroundColor Cyan
