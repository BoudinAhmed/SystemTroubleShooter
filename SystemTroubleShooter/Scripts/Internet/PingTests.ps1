# Define the targets for ping tests
$targets = @("127.0.0.1", "8.8.8.8", "google.com")

# Get the default gateway dynamically
$defaultGateway = (Get-NetIPConfiguration | Where-Object { $_.IPv4DefaultGateway -ne $null }).IPv4DefaultGateway.NextHop
if ($defaultGateway) {
    $targets += $defaultGateway
} else {
    Write-Output "No default gateway found."
}

# Track failures
$failedPings = 0

# Perform ping tests
foreach ($target in $targets) {
    Write-Output "Pinging $target..."
    
    # Run Test-Connection and check if it returns a valid response
    $pingResult = Test-Connection -ComputerName $target -Count 1 -ErrorAction SilentlyContinue

    if ($pingResult) {
        Write-Output "Ping to $target successful."
    } else {
        Write-Output "Ping to $target failed."
        $failedPings++
    }
}

# Final result
if ($failedPings -eq 0) {
    
    exit 0
} else {
    
    exit 1
}