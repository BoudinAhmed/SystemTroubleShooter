Function Clear-WindowsUpdateCache {
    [CmdletBinding()]
    Param()
    Process {
        Write-Host "Stopping Windows Update services..." -ForegroundColor Cyan
        Stop-Service -Name "wuauserv", "bits", "cryptsvc" -Force -ErrorAction SilentlyContinue | Out-Null
        Start-Sleep -Seconds 3 # Give services time to stop

        $softwareDistributionPath = "$env:SystemRoot\SoftwareDistribution"
        $catroot2Path = "$env:SystemRoot\System32\catroot2"

        if (Test-Path $softwareDistributionPath) {
            Write-Host "Renaming SoftwareDistribution folder to SoftwareDistribution.old..." -ForegroundColor Yellow
            Rename-Item -Path $softwareDistributionPath -NewName "$($softwareDistributionPath).old" -Force -ErrorAction SilentlyContinue
        } else {
            Write-Warning "SoftwareDistribution folder not found at $softwareDistributionPath."
        }

        # Optional: Rename Catroot2. This is less frequently needed but can help with signature issues.
        if (Test-Path $catroot2Path) {
            Write-Host "Renaming Catroot2 folder to Catroot2.old..." -ForegroundColor Yellow
            Rename-Item -Path $catroot2Path -NewName "$($catroot2Path).old" -Force -ErrorAction SilentlyContinue
        } else {
            Write-Warning "Catroot2 folder not found at $catroot2Path."
        }

        Write-Host "Starting Windows Update services..." -ForegroundColor Cyan
        Start-Service -Name "wuauserv", "bits", "cryptsvc" -ErrorAction SilentlyContinue | Out-Null

        Write-Host "Windows Update cache cleared. Please try checking for updates again." -ForegroundColor Green
    }
}
#Running the Function
Clear-WindowsUpdateCache