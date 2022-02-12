

#Clean previous reports
Get-ChildItem -Path .\docker\data\tests -Exclude ".gitkeep" -Recurse | Remove-Item -Force -Recurse -Verbose


#Container list of tests
$tests =@(
    @("docker-compose up -d", "docker-compose down", "Default Dianoga async setup"),
    @("docker-compose -f docker-compose.yml -f docker-compose.override.sync.yml up -d", "docker-compose -f docker-compose.yml -f docker-compose.override.sync.yml down","Default Dianoga sync setup"),
    @("docker-compose -f docker-compose.yml -f docker-compose.override.svg.yml up -d", "docker-compose -f docker-compose.yml -f docker-compose.override.svg.yml down","Default Dianoga async setup with SVG enabled"),
    @("docker-compose -f docker-compose.yml -f docker-compose.override.svg.webp.yml up -d", "docker-compose -f docker-compose.yml -f docker-compose.override.svg.webp.yml down","Default Dianoga async setup with SVG & WebP enabled"),
    @("docker-compose -f docker-compose.yml -f docker-compose.override.svg.webp.avif.jxl.yml up -d", "docker-compose -f docker-compose.yml -f docker-compose.override.svg.webp.avif.jxl.yml down","Default Dianoga async setup with SVG & WebP & Avif & JPEG XL enabled")
)

$output = ""

For($i = 0; $i -lt $tests.Length; $i++) {
    $testName = $tests[$i][2]
    #Clean MediaCache
    Get-ChildItem -Path .\docker\data\mediaCache -Exclude ".gitkeep" -Recurse | Remove-Item -Force -Recurse -Verbose

    $datetime = Get-Date -Format "HH:mm:ss "
    Write-Host "$datetime Starting $testName". 

    Invoke-Expression $tests[$i][0]

    $datetime = Get-Date -Format "HH:mm:ss "

    Write-Host "$datetime $testName was started"
    #docker container ls -f Name=dianoga_test
    while((docker container ls -f name=dianoga_test -q ) -ne $null)
    {
        Start-Sleep -Seconds 30
        $datetime = Get-Date -Format "HH:mm:ss "
        Write-Host "$datetime Waiting for dianoga_test container to finish testing"
    }

    Invoke-Expression $tests[$i][1]

    $report = Get-ChildItem .\docker\data\tests | sort creationtime -Descending |  Select-Object -first 1
    $result = Get-Content $report.FullName -Tail 1 

    $output += "Test results for: $testName`n"
    $output += $result
    $output += "`n======================================"
}

Write-Host $output



