
# Geting all network adapters
$networkAdapters = Get-NetAdapter

foreach ($adapter in $networkAdapters) {
    # Disable the network adapter
    Disable-NetAdapter -Name $adapter.Name -Confirm:$false
    Write-Output "Network adapter '$($adapter.Name)' disabled."

    # Wait for a few seconds
    Start-Sleep -Seconds 5

    # Reenable the network adapter
    Enable-NetAdapter -Name $adapter.Name -Confirm:$false
    Write-Output "Network adapter '$($adapter.Name
