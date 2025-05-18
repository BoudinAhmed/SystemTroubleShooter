try{
$adapter = Get-NetAdapter | Where-Object {$_.Status -eq 'Up' -and $_.HardwareInterface -eq $true}

if($adapter.Count -eq 0){
	
	Write-Output "No active adapters found"
	Start-Sleep -Seconds 1
	exit 1 
}


Write-Output $adapter.name
}
catch{
	Write-Host $_.Exception.Message
}
