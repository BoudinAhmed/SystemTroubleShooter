param(
    [Parameter(Mandatory=$true)]
    [string] $drivesString
)

Write-Host "Received drives string: $($drivesString)"

if ([string]::IsNullOrWhiteSpace($drivesString)) {
    Write-Warning "No network drive information provided (parameter is null or empty)."
    return 1
}

if ($drivesString) {
    $drivePairs = $drivesString.Split(';')
    foreach ($pair in $drivePairs) {
        $parts = $pair.Split(',')
        if ($parts.Count -eq 2) {
            $driveLetter = $parts[0]
            $networkPath = $parts[1]
            $commandArg = "${driveLetter} $networkPath" # Corrected line
            Write-Host "Attempting to map: $($commandArg)"
            Invoke-Expression "net use $commandArg"
            Write-Host "Result of net use: $($LASTEXITCODE)" # Check the exit code
        } else {
            Write-Warning "Invalid drive mapping format: $pair"
        }
    }
} else {
    Write-Warning "No network drive information provided."
}