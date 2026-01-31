# Blinkt API (.NET)

ASP.NET Core Web API for controlling Pimoroni Blinkt LED strip.

**Port of:** Python Flask version  
**Target:** Pi Zero W (ARM64)  
**Port:** 5001

## Architecture

- **ASP.NET Core 9 Minimal API**
- **IHostedService** background animation player
- **Singleton AnimationController** for state management
- **JSON animation definitions** (compatible with Python version)

## Status

🚧 **Work in Progress**

- [x] Project scaffolding
- [x] Basic API endpoints (/, /status, /sequence, /animations)
- [x] AnimationController service
- [x] AnimationPlayer background service
- [ ] GPIO/Blinkt hardware integration
- [ ] Animation JSON loader
- [ ] Animation rendering engine
- [ ] All 35 animations ported

## Next Steps

1. Add GPIO library (Iot.Device.Bindings or PInvoke to blinkt C lib)
2. Port animation JSON files
3. Implement animation rendering
4. Deploy to Pi Zero W
5. Create systemd service

