    $audioDevices = Get-PnpDevice -Class "AudioEndpoint"
    if ($audioDevices.Count -gt 0) {
        $audioDevices | Format-Table FriendlyName -AutoSize
        exit 0
    } else {
        Write-Warning "No audio endpoint devices found."
        exit 1
    }