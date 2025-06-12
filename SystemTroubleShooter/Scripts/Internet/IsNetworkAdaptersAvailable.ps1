try{
$adapter = Get-NetAdapter | Where-Object {$_.Status -eq 'Up' -and $_.HardwareInterface -eq $true}

if($adapter.Count -eq 0){
	Write-Output "No network adapters found"
	Write-Output "FINAL:No network adapters found. Please ensure your Ethernet cable is securely connected or verify that your Wi-Fi is enabled. If you're using a router, check that it's powered on and functioning properly before retrying."
	Start-Sleep -Seconds 1
	exit 1 
}


Write-Output $adapter.name
}
catch{
	Write-Host $_.Exception.Message
}
