#Get Adapter
$adapter = Get-NetAdapter | Where-Object {$_.Status -eq 'Up' -and $_.HardwareInterface -eq $true}
$adapterName = $adapter.Name
$ipConfig = Get-NetIPConfiguration -InterfaceAlias $adapterName -ErrorAction SilentlyContinue

Disable-NetAdapter -Name $adapterName -Confirm: $false
Write-Output = $ipConfig 

# Enable the adapter
Enable-NetAdapter -Name $adapterName -Confirm:$false
Write-Output = $ipConfig 
Write-Output "Reinitializing $adapterName"
Start-Sleep -Seconds 10


