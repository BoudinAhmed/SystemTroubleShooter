$audioDevices = Get-PnpDevice -Class "AudioEndpoint" | Where-Object {$_.FriendlyName -ne $null -and $_.FriendlyName -ne ""}

 $inputDevices = @()
 $outputDevices = @()

 foreach($device in $audioDevices){
     if($device.InstanceId -match "0.0.1.00000000"){
        $inputDevices += $device.FriendlyName
     }
     else{
         $outputDevices += $device.FriendlyName
     }
 }

$result = @{
    InputDevices = $inputDevices | Sort-Object  
    OutputDevices = $outputDevices | Sort-Object 
}


$result | ConvertTo-Json -Compress
exit 0 
