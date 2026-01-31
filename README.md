# Blinkt API (.NET)

ASP.NET Core Web API for controlling Pimoroni Blinkt LED strip.

**Port of:** Python Flask version  
**Target:** Pi Zero W (ARM64)  
**Port:** 5001

## Architecture

- **ASP.NET Core 9 Minimal API** - lightweight, fast startup
- **IHostedService** background animation player (~50 FPS render loop)
- **Singleton AnimationController** for state management
- **System.Device.Gpio** - native SPI/APA102 protocol
- **JSON animation definitions** (28 animations ported)
- **Self-contained deployment** - single executable, no runtime needed

## Features

✅ **Full animation engine**
- 7 animation types: global_oscillator, pixel_oscillator, traveling_wave, scanner, fill, sparkle, rainbow_cycle
- 28 animations: pulse, shimmer, wave, fire, rainbow, heartbeat, thumbs-up, etc.
- Animation stacking (finite-duration animations auto-return to previous)
- Per-pixel control with brightness
- Color parsing (10 named colors)

✅ **Hardware integration**
- APA102 LED protocol over SPI
- 8 RGB LEDs with 5-bit brightness
- Graceful fallback if no SPI device (test mode)

✅ **API endpoints**
- `GET /` - status
- `GET /status` - current animation state
- `GET /animations` - list all animations
- `GET /animations/{name}` - animation details
- `POST /sequence` - start animation

## Build

```bash
# Debug build
dotnet build

# Release (self-contained ARM64)
dotnet publish -c Release
```

## Deploy to Pi Zero W

```bash
# Copy the single executable
scp bin/Release/net9.0/linux-arm64/publish/BlinktApi pi@pizero:/home/pi/blinkt-api/

# SSH in and run
ssh pi@pizero
cd blinkt-api
chmod +x BlinktApi
./BlinktApi
```

## API Usage

```bash
# Start shimmer animation (cyan, infinite)
curl -X POST http://pizero:5001/sequence \
  -H "Content-Type: application/json" \
  -d '{"name": "shimmer", "color": "cyan"}'

# Pulse (green, 3 seconds)
curl -X POST http://pizero:5001/sequence \
  -H "Content-Type: application/json" \
  -d '{"name": "pulse", "color": "green", "duration": 3}'

# Get status
curl http://pizero:5001/status

# List animations
curl http://pizero:5001/animations
```

## Animation Types

### global_oscillator
All LEDs oscillate together (pulse, heartbeat)

### pixel_oscillator  
Each LED oscillates independently (shimmer, fire)

### traveling_wave
Wave moves across LEDs (wave, scanner)

### scanner
KITT-style back-and-forth scan

### fill
LEDs fill up sequentially

### sparkle
Random sparkle effect

### rainbow_cycle
Rotating rainbow gradient

## Performance

- ~50 FPS render loop
- ~2MB memory footprint
- Self-contained binary (~60MB trimmed)
- No Python interpreter overhead

## Next Steps

- [ ] Add more animation types (explosion, star, etc.)
- [ ] Systemd service
- [ ] HTTPS support
- [ ] WebSocket streaming
- [ ] Animation composition/layers

## License

MIT
