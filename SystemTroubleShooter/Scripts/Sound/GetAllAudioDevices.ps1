$audioDevices = Get-PnpDevice -Class "AudioEndpoint" | Where-Object {$_.FriendlyName -ne $null -and $_.FriendlyName -ne ""}

if ($audioDevices) {
    # Output each FriendlyName on a new line
    $audioDevices.FriendlyName
    exit 0
} else {
    Write-Warning "No audio endpoint devices found."
    exit 1
}