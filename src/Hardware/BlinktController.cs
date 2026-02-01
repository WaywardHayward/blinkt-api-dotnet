using System.Device.Gpio;
using Microsoft.Extensions.Logging;

namespace BlinktApi.Hardware;

/// <summary>
/// Pimoroni Blinkt LED strip controller (8 RGB LEDs)
/// Uses APA102 protocol over GPIO bit-banging (GPIO 23 = DAT, GPIO 24 = CLK)
/// </summary>
public class BlinktController : IDisposable
{
    private const int PixelCount = 8;
    private const int DAT = 23; // GPIO 23 (Physical pin 16)
    private const int CLK = 24; // GPIO 24 (Physical pin 18)
    
    private readonly GpioController? _gpio;
    private readonly byte[] _pixels = new byte[PixelCount * 4]; // Brightness + BGR for each pixel
    private readonly ILogger<BlinktController>? _logger;
    private bool _disposed;
    
    public bool IsHardwareAvailable => _gpio != null;

    public BlinktController(ILogger<BlinktController>? logger = null)
    {
        _logger = logger;
        
        try
        {
            _gpio = new GpioController();
            _gpio.OpenPin(DAT, PinMode.Output);
            _gpio.OpenPin(CLK, PinMode.Output);
            _gpio.Write(DAT, PinValue.Low);
            _gpio.Write(CLK, PinValue.Low);
            _logger?.LogInformation("Blinkt hardware initialized successfully on GPIO {DAT}/{CLK}", DAT, CLK);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Could not initialize GPIO. Running in test mode (no hardware)");
            _gpio?.Dispose();
            _gpio = null;
        }
    }

    public void SetPixel(int index, byte red, byte green, byte blue, double brightness = 1.0)
    {
        if (index < 0 || index >= PixelCount)
        {
            _logger?.LogError("SetPixel: Invalid index {Index}. Must be 0-{MaxIndex}", index, PixelCount - 1);
            throw new ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {PixelCount - 1}");
        }

        if (brightness < 0 || brightness > 1)
        {
            _logger?.LogError("SetPixel: Invalid brightness {Brightness}. Must be 0.0-1.0", brightness);
            throw new ArgumentOutOfRangeException(nameof(brightness), "Brightness must be between 0.0 and 1.0");
        }

        var offset = index * 4;
        _pixels[offset] = (byte)(0b11100000 | (byte)(brightness * 31)); // Brightness (5-bit)
        _pixels[offset + 1] = blue;
        _pixels[offset + 2] = green;
        _pixels[offset + 3] = red;
        
        _logger?.LogDebug("SetPixel({Index}): R={Red} G={Green} B={Blue} Brightness={Brightness:F2}", 
            index, red, green, blue, brightness);
    }

    public void SetAll(byte red, byte green, byte blue, double brightness = 1.0)
    {
        for (int i = 0; i < PixelCount; i++)
        {
            SetPixel(i, red, green, blue, brightness);
        }
    }

    public void Clear()
    {
        // APA102 requires brightness byte to be 0b111xxxxx format
        // Setting all to 0 isn't enough - need proper brightness byte with 0 RGB
        for (int i = 0; i < PixelCount; i++)
        {
            SetPixel(i, 0, 0, 0, 0.0);
        }
    }

    public void Show()
    {
        if (_gpio == null)
        {
            _logger?.LogWarning("Show() called but GPIO is null - running in test mode");
            return; // Test mode - no hardware
        }

        try
        {
            _logger?.LogDebug("Writing {PixelCount} pixels via GPIO bit-banging", PixelCount);
            
            // Start of Frame (SOF): 32 clock pulses with DAT low
            WriteStartFrame();
            
            // Pixel data (4 bytes per pixel: brightness, blue, green, red)
            for (int i = 0; i < _pixels.Length; i++)
            {
                WriteByte(_pixels[i]);
            }
            
            // End of Frame (EOF): 36 clock pulses with DAT low
            // (Blinkt uses specific APA102 chips that need 36 clocks to latch)
            WriteEndFrame();
            
            _logger?.LogDebug("GPIO write completed successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to write to GPIO");
            throw new InvalidOperationException("Failed to update LED strip", ex);
        }
    }

    private void WriteStartFrame()
    {
        // SOF: 32 clock pulses with DAT=0
        _gpio!.Write(DAT, PinValue.Low);
        for (int i = 0; i < 32; i++)
        {
            _gpio.Write(CLK, PinValue.High);
            _gpio.Write(CLK, PinValue.Low);
        }
    }

    private void WriteEndFrame()
    {
        // EOF: 36 clock pulses with DAT=0
        // Blinkt's specific APA102 chips need 36 clocks (not the standard 4)
        _gpio!.Write(DAT, PinValue.Low);
        for (int i = 0; i < 36; i++)
        {
            _gpio.Write(CLK, PinValue.High);
            _gpio.Write(CLK, PinValue.Low);
        }
    }

    private void WriteByte(byte value)
    {
        // Bit-bang the byte MSB first
        for (int i = 7; i >= 0; i--)
        {
            var bit = (value & (1 << i)) != 0;
            _gpio!.Write(DAT, bit ? PinValue.High : PinValue.Low);
            _gpio.Write(CLK, PinValue.High);
            _gpio.Write(CLK, PinValue.Low);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        try
        {
            Clear();
            Show();
            
            if (_gpio != null)
            {
                _gpio.Write(DAT, PinValue.Low);
                _gpio.Write(CLK, PinValue.Low);
                _gpio.ClosePin(DAT);
                _gpio.ClosePin(CLK);
                _gpio.Dispose();
            }
            
            _logger?.LogInformation("Blinkt hardware disposed successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error disposing Blinkt hardware");
        }
        finally
        {
            _disposed = true;
        }
    }
}
