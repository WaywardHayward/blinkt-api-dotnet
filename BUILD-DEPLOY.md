# Blinkt .NET API - Build & Deploy Guide

## Key Learnings

### Hardware Configuration
- **Blinkt HAT uses GPIO bit-banging, NOT hardware SPI**
  - DAT pin: GPIO 23 (Physical pin 16)
  - CLK pin: GPIO 24 (Physical pin 18)
  - Hardware SPI (GPIO 10/11) will NOT work with Blinkt
  - Must use `System.Device.Gpio`, not `System.Device.Spi`

### Architecture
- **Pi Zero 2 W is 64-bit (aarch64)**
  - Build target: `linux-arm64`
  - NOT `linux-arm` (32-bit)
  - Check with: `ssh pi@192.168.4.90 'uname -m'`

### Build Environment
- **Build on main Pi (raspberrypi), deploy to Pi Zero**
  - Main Pi has .NET SDK installed at `~/.dotnet`
  - Pi Zero's .NET SDK is broken (missing libhostfxr)
  - Always build on main Pi and SCP to Pi Zero

## Build & Deploy Workflow

### 1. Prerequisites
Ensure .NET SDK is in PATH on main Pi:
```bash
export PATH="$HOME/.dotnet:$PATH"
dotnet --version  # Should show 9.0.310
```

### 2. Build for Pi Zero (64-bit ARM)
From main Pi:
```bash
cd ~/dev/blinkt-api-dotnet
export PATH="$HOME/.dotnet:$PATH"
dotnet publish src -c Release -r linux-arm64 --self-contained -o publish/linux-arm64
```

**Build output:**
- Creates self-contained executable
- Includes all .NET runtime dependencies
- Located at: `publish/linux-arm64/BlinktApi`

### 3. Stop Service on Pi Zero
Service must be stopped before deploying (file locks):
```bash
ssh pi@192.168.4.90 'sudo systemctl stop blinkt-api-dotnet'
```

### 4. Deploy to Pi Zero
```bash
scp -r ~/dev/blinkt-api-dotnet/publish/linux-arm64/* pi@192.168.4.90:/home/pi/blinkt-api/
```

**Important:** This overwrites everything in `/home/pi/blinkt-api/` including:
- Config files (appsettings.json)
- Animation files

### 5. Start Service
```bash
ssh pi@192.168.4.90 'sudo systemctl start blinkt-api-dotnet'
```

### 6. Verify & Test
Check service status:
```bash
ssh pi@192.168.4.90 'sudo systemctl status blinkt-api-dotnet'
```

Test the API:
```bash
curl -X POST http://192.168.4.90:5001/sequence \
  -H "Content-Type: application/json" \
  -d '{"name": "pulse", "color": "magenta", "duration": 5}'
```

## Common Issues & Solutions

### Issue: Service won't start (Status 203/EXEC)
**Cause:** Wrong architecture (built for 32-bit, Pi Zero 2 W is 64-bit)
**Solution:** Rebuild with `-r linux-arm64`

### Issue: LEDs not responding
**Cause 1:** Using hardware SPI instead of GPIO bit-banging
**Solution:** Check `BlinktController.cs` uses `GpioController` on pins 23/24

**Cause 2:** Blinkt physically disconnected
**Solution:** Power off, reseat HAT, power on

### Issue: File locked during deployment
**Cause:** Service is running
**Solution:** `ssh pi@192.168.4.90 'sudo systemctl stop blinkt-api-dotnet'`

### Issue: Config changes lost after deploy
**Cause:** `appsettings.json` gets overwritten during SCP
**Solution:** Either:
1. Copy config separately after deploy, OR
2. Update source `src/appsettings.json` before building

## Service Details

**Location:** `/etc/systemd/system/blinkt-api-dotnet.service`
**User:** `pi`
**WorkingDirectory:** `/home/pi/blinkt-api`
**ExecStart:** `/home/pi/blinkt-api/BlinktApi`
**Auto-start:** Enabled

**Logs:**
```bash
ssh pi@192.168.4.90 'sudo journalctl -u blinkt-api-dotnet -f'
```

## API Endpoints

- **Swagger UI:** http://192.168.4.90:5001/swagger/index.html
- **Animations:** `POST /sequence` with `{"name": "animation", "color": "color", "duration": seconds}`

## Quick One-Liner Deploy

From main Pi:
```bash
cd ~/dev/blinkt-api-dotnet && \
export PATH="$HOME/.dotnet:$PATH" && \
dotnet publish src -c Release -r linux-arm64 --self-contained -o publish/linux-arm64 && \
ssh pi@192.168.4.90 'sudo systemctl stop blinkt-api-dotnet' && \
scp -r publish/linux-arm64/* pi@192.168.4.90:/home/pi/blinkt-api/ && \
ssh pi@192.168.4.90 'sudo systemctl start blinkt-api-dotnet && sleep 3 && curl -X POST http://localhost:5001/sequence -H "Content-Type: application/json" -d '"'"'{"name":"pulse","color":"green","duration":3}'"'"''
```

This will build, deploy, restart, and test with a green pulse.

---

**Last Updated:** 2026-02-01
**Maintained By:** Rabbie Pi 🐑
