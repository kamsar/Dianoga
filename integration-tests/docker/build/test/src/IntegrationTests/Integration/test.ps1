$datetime = Get-Date -Format "yyyyMMdd-HHmmss"
$prefix = $env:TestConfigurationName
$suffix = ".txt"
$fileName = $prefix + "_" + $datetime + $suffix

Write-Host $env:CDHostname
Write-Host $env:TestConfigurationName

dotnet test Integration.csproj > ..\..\results\$fileName