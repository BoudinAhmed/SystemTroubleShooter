$adapter = Get-NetAdapter | Where-Object {$_.Status -eq 'Up' -and $_.HardwareInterface -eq $true}

if($adapter.Count -eq 0){

	Write-Output "No active network adapters found"
	exit 1 
}

Write-Output "Active network adapters found: $($adapter.name -join ', ')"

