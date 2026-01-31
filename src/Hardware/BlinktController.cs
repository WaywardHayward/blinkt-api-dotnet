using System.Device.Gpio;
using System.Device.Spi;
using Microsoft.Extensions.Logging;

namespace BlinktApi.Hardware;

/// <summary>
/// Pimoroni Blinkt LED strip controller (8 RGB LEDs)
/// Uses APA102 protocol over SPI
/// </summary>
public class BlinktController : IDisposable
{
    private const int PixelCount = 8;
    private readonly SpiDevice? _spiDevice;
    private readonly byte[] _pixels = new byte[PixelCount * 4]; // BGRA for each pixel
    private readonly ILogger<BlinktController>? _logger;
    private bool _disposed;
    
    public bool IsHardwareAvailable => _spiDevice != null;

    public BlinktController(ILogger<BlinktController>? logger = null)
    {
        _logger = logger;
        
        try
        {
            var settings = new SpiConnectionSettings(0, 0)
            {
                ClockFrequency = 8_000_000, // 8 MHz
                Mode = SpiMode.Mode0
            };
            _spiDevice = SpiDevice.Create(settings);
            _logger?.LogInformation("Blinkt hardware initialized successfully on SPI 0.0");
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Could not initialize SPI device. Running in test mode (no hardware)");
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
        Array.Clear(_pixels);
    }

    public void Show()
    {
        if (_spiDevice == null)
        {
            _logger?.LogDebug("Show() called in test mode - no hardware available");
            return; // Test mode - no hardware
        }

        try
        {
            // APA102 protocol: Start frame, pixel data, end frame
            var buffer = new byte[4 + _pixels.Length + 4];
            
            // Start frame (32 bits of 0)
            // buffer[0..3] already 0 from initialization
            
            // Pixel data
            _pixels.CopyTo(buffer, 4);
            
            // End frame (32 bits of 1) - actually just need clock pulses
            buffer[^4] = 0xFF;
            buffer[^3] = 0xFF;
            buffer[^2] = 0xFF;
            buffer[^1] = 0xFF;
            
            _spiDevice.Write(buffer);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to write to SPI device");
            throw new InvalidOperationException("Failed to update LED strip", ex);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        try
        {
            Clear();
            Show();
            _spiDevice?.Dispose();
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
