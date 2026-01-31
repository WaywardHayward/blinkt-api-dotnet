# Deploy Blinkt .NET API to Pi Zero W
# Run this from the blinkt-api-dotnet directory on Windows

$ErrorActionPreference = "Stop"

$piHost = "pi@192.168.4.90"
$piPassword = "Viltvodel6!"
$deployPath = "/home/pi/blinkt-api-dotnet"

Write-Host "Building self-contained .NET app for linux-arm64..." -ForegroundColor Cyan

# Build self-contained for ARM64
dotnet publish src/BlinktApi.csproj `
    --configuration Release `
    --runtime linux-arm64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:PublishTrimmed=true `
    --output ./publish

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Build complete. Package size:" -ForegroundColor Green
Get-ChildItem ./publish | Measure-Object -Property Length -Sum | Select-Object Count, @{Name="SizeMB";Expression={[math]::Round($_.Sum/1MB,2)}}

Write-Host "`nCopying to Pi Zero W..." -ForegroundColor Cyan

# Use SCP to copy files (requires scp.exe on Windows)
# You may need to install OpenSSH Client from Windows Features
scp -r ./publish/* ${piHost}:${deployPath}/

Write-Host "`nSetting up systemd service on Pi Zero..." -ForegroundColor Cyan

# Create systemd service file
$serviceContent = @"
[Unit]
Description=Blinkt .NET API
After=network.target

[Service]
Type=simple
User=pi
WorkingDirectory=$deployPath
ExecStart=$deployPath/BlinktApi
Restart=always
RestartSec=5
StandardOutput=journal
StandardError=journal

[Install]
WantedBy=multi-user.target
"@

# Write service file locally
$serviceContent | Out-File -FilePath ./blinkt-api-dotnet.service -Encoding utf8

# Copy and install service
scp ./blinkt-api-dotnet.service ${piHost}:/tmp/
Remove-Item ./blinkt-api-dotnet.service

# SSH commands to set up service
$sshCommands = @"
sudo mv /tmp/blinkt-api-dotnet.service /etc/systemd/system/
sudo chmod 644 /etc/systemd/system/blinkt-api-dotnet.service
cd $deployPath && chmod +x BlinktApi
sudo systemctl daemon-reload
sudo systemctl enable blinkt-api-dotnet
sudo systemctl stop blinkt-api  # Stop old Python service if running
sudo systemctl start blinkt-api-dotnet
sudo systemctl status blinkt-api-dotnet --no-pager
"@

ssh ${piHost} $sshCommands

Write-Host "`nDeployment complete!" -ForegroundColor Green
Write-Host "API should be running at http://192.168.4.90:5001" -ForegroundColor Cyan
Write-Host "Swagger UI at http://192.168.4.90:5001/swagger" -ForegroundColor Cyan
