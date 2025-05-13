#Get Adapter
$adapter = Get-NetAdapter | Where-Object {$_.Status -eq 'Up' -and $_.HardwareInterface -eq $true}
$adapterName = $adapter.Name

Disable-NetAdapter -Name $adapterName -Confirm:$false
Start-Sleep -Seconds 1

# Enable the adapter
Enable-NetAdapter -Name $adapterName -Confirm:$false

Write-Output "Network adapter '$adapterName' restarted successfully."

