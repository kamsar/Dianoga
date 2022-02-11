#Clean MediaCache
Get-ChildItem -Path .\docker\data\mediaCache -Exclude ".gitkeep" -Recurse | Remove-Item -Force -Recurse -Verbose

#Clean previous reports
Get-ChildItem -Path .\docker\data\tests -Exclude ".gitkeep" -Recurse | Remove-Item -Force -Recurse -Verbose

$datetime = Get-Date -Format "HH:mm:ss "
Write-Host "$datetime Starting Default Dianoga setup"

docker-compose up -d    

$datetime = Get-Date -Format "HH:mm:ss "
Write-Host "$datetime Default Dianoga setup was started"
#docker container ls -f Name=dianoga_test
while((docker container ls -f name=dianoga_test -q ) -ne $null)
{
    Start-Sleep -Seconds 30
    $datetime = Get-Date -Format "HH:mm:ss "
    Write-Host "$datetime Waiting for dianoga_test container to finish testing"
}

docker-compose down
