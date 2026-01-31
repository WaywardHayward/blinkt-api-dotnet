using System.Device.Gpio;
using System.Device.Spi;

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
    private bool _disposed;

    public BlinktController()
    {
        try
        {
            var settings = new SpiConnectionSettings(0, 0)
            {
                ClockFrequency = 8_000_000, // 8 MHz
                Mode = SpiMode.Mode0
            };
            _spiDevice = SpiDevice.Create(settings);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not initialize SPI device: {ex.Message}");
            Console.WriteLine("Running in test mode (no hardware)");
        }
    }

    public void SetPixel(int index, byte red, byte green, byte blue, double brightness = 1.0)
    {
        if (index < 0 || index >= PixelCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (brightness < 0 || brightness > 1)
            throw new ArgumentOutOfRangeException(nameof(brightness));

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
            return; // Test mode - no hardware

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

    public void Dispose()
    {
        if (_disposed) return;
        
        Clear();
        Show();
        _spiDevice?.Dispose();
        _disposed = true;
    }
}
