param(
[Parameter](Mandatory=$true)
[array] $drives)

foreach($drive in $drives){
	$commandArg = $drive[0] + ": " + $drive[1]
	Invoke-Expression "net use $commandArg"
}

