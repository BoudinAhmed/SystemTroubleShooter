    $services = @("audiosrv", "AudioEndpointBuilder")
    foreach ($service in $services) {
        $svc = Get-Service -Name $service -ErrorAction SilentlyContinue
        if ($svc) {
             if ($svc.Status -ne "Running") {
                Write-Host "Attempting to start service '$($svc.DisplayName)'..."
                try {
                    Start-Service -Name $service -Force -ErrorAction Stop
                    Write-Host "Service '$($svc.DisplayName)' started successfully."
                } catch {
                    Write-Error "Failed to start service '$($svc.DisplayName)': $($_.Exception.Message)"
                }
            } else {
                Write-Host "Service '$($svc.DisplayName)' is already running."
            }
        } else {
            exit 1
        }
    }
