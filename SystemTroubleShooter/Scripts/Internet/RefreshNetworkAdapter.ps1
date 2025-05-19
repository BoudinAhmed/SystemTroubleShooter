#Get Adapter
$Adapter = Get-NetAdapter | Where-Object {$_.Status -eq 'Up' -and $_.HardwareInterface -eq $true}
$AdapterName = $Adapter.Name

# Diaable the adapter
Disable-NetAdapter -Name $AdapterName -Confirm: $false
Start-Sleep -Seconds 1

# Enable the adapter
Enable-NetAdapter -Name $AdapterName -Confirm:$false

#Waiting For adapter to be UP
Write-Output "Reinitializing $AdapterName"

$TimeoutSeconds = 30
$CheckIntervalSeconds = 1
$EnableWaitInitialSeconds = 2
$startTime = Get-Date

do{
	$currentAdapter = Get-NetAdapter -Name $AdapterName -ErrorAction SilentlyContinue

	if ($currentAdapter -and $currentAdapter.Status -eq 'Up') {
	Write-Output "$AdapterName Up"
	exit 0
	}

	$elapsedSeconds = ((Get-Date) - $startTime).TotalSeconds

	if ($elapsedSeconds -ge $TimeoutSeconds){
		Write-Output "waiting for adapter timed out"
		exit 1
	}
	Start-Sleep -Seconds $CheckIntervalSeconds
}
while($true)


