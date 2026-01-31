# Blinkt .NET API - Deployment Guide

## Prerequisites
- .NET 9 SDK installed on your Windows machine
- SSH/SCP access to Pi Zero W (192.168.4.90)

## Option 1: Quick Deploy (PowerShell)

Run from the `blinkt-api-dotnet` directory:

```powershell
# Build
dotnet publish src/BlinktApi.csproj `
    -c Release `
    -r linux-arm64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:PublishTrimmed=true `
    -o ./publish

# Deploy (use WinSCP, scp, or rsync)
scp -r ./publish/* pi@192.168.4.90:/home/pi/blinkt-api-dotnet/

# Set up service
ssh pi@192.168.4.90 "chmod +x /home/pi/blinkt-api-dotnet/BlinktApi && sudo systemctl restart blinkt-api-dotnet"
```

Password: `Viltvodel6!`

## Option 2: Use deploy.ps1 Script

```powershell
.\deploy.ps1
```

## Systemd Service

The service file is at `/etc/systemd/system/blinkt-api-dotnet.service`:

```ini
[Unit]
Description=Blinkt .NET API
After=network.target

[Service]
Type=simple
User=pi
WorkingDirectory=/home/pi/blinkt-api-dotnet
ExecStart=/home/pi/blinkt-api-dotnet/BlinktApi
Restart=always
RestartSec=5
StandardOutput=journal
StandardError=journal

[Install]
WantedBy=multi-user.target
```

## First-Time Setup

If the service doesn't exist yet:

```bash
ssh pi@192.168.4.90

# Create deploy directory
mkdir -p ~/blinkt-api-dotnet

# Create systemd service
sudo tee /etc/systemd/system/blinkt-api-dotnet.service << 'EOF'
[Unit]
Description=Blinkt .NET API
After=network.target

[Service]
Type=simple
User=pi
WorkingDirectory=/home/pi/blinkt-api-dotnet
ExecStart=/home/pi/blinkt-api-dotnet/BlinktApi
Restart=always
RestartSec=5
StandardOutput=journal
StandardError=journal

[Install]
WantedBy=multi-user.target
EOF

# Enable and start
sudo systemctl daemon-reload
sudo systemctl enable blinkt-api-dotnet
```

## After Deployment

```bash
# Check status
ssh pi@192.168.4.90 "sudo systemctl status blinkt-api-dotnet"

# View logs
ssh pi@192.168.4.90 "sudo journalctl -u blinkt-api-dotnet -f"

# Test API
curl http://192.168.4.90:5001/status
curl http://192.168.4.90:5001/animations
```

## Replacing Python Version

If you want to stop the old Python service:

```bash
ssh pi@192.168.4.90 "sudo systemctl stop blinkt-api && sudo systemctl disable blinkt-api"
```
