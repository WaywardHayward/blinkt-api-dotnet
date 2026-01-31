using System.Drawing;
using BlinktApi.Hardware;
using BlinktApi.Models;

namespace BlinktApi.Rendering;

public interface IAnimationRenderer
{
    void Render(BlinktController blinkt, Color color, double elapsedSeconds);
}

public class GlobalOscillatorRenderer : IAnimationRenderer
{
    private readonly double _periodSeconds;
    private readonly double _minBrightness;
    private readonly double _maxBrightness;
    private readonly string _oscillator;

    public GlobalOscillatorRenderer(double periodSeconds, double minBrightness, double maxBrightness, string oscillator)
    {
        _periodSeconds = periodSeconds;
        _minBrightness = minBrightness;
        _maxBrightness = maxBrightness;
        _oscillator = oscillator;
    }

    public void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        var phase = (elapsedSeconds / _periodSeconds) * 2 * Math.PI;
        var brightness = _oscillator switch
        {
            "sine" => SineWave(phase),
            "square" => SquareWave(phase),
            "triangle" => TriangleWave(phase),
            _ => SineWave(phase)
        };
        
        brightness = _minBrightness + (brightness * (_maxBrightness - _minBrightness));
        blinkt.SetAll(color.R, color.G, color.B, brightness);
        blinkt.Show();
    }

    private static double SineWave(double phase) => (Math.Sin(phase) + 1) / 2;
    private static double SquareWave(double phase) => Math.Sin(phase) >= 0 ? 1.0 : 0.0;
    private static double TriangleWave(double phase) => 1.0 - Math.Abs((phase % (2 * Math.PI)) / Math.PI - 1.0);
}

public class PixelOscillatorRenderer : IAnimationRenderer
{
    private readonly double _minBrightness;
    private readonly double _maxBrightness;
    private readonly double _minSpeed;
    private readonly double _maxSpeed;
    private readonly Random _random = new();
    private readonly double[] _phases;
    private readonly double[] _speeds;

    public PixelOscillatorRenderer(double minBrightness, double maxBrightness, double minSpeed, double maxSpeed)
    {
        _minBrightness = minBrightness;
        _maxBrightness = maxBrightness;
        _minSpeed = minSpeed;
        _maxSpeed = maxSpeed;
        
        _phases = new double[8];
        _speeds = new double[8];
        for (int i = 0; i < 8; i++)
        {
            _phases[i] = _random.NextDouble() * 2 * Math.PI;
            _speeds[i] = _minSpeed + _random.NextDouble() * (_maxSpeed - _minSpeed);
        }
    }

    public void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        for (int i = 0; i < 8; i++)
        {
            _phases[i] += _speeds[i];
            var brightness = (Math.Sin(_phases[i]) + 1) / 2;
            brightness = _minBrightness + (brightness * (_maxBrightness - _minBrightness));
            blinkt.SetPixel(i, color.R, color.G, color.B, brightness);
        }
        blinkt.Show();
    }
}

public class TravelingWaveRenderer : IAnimationRenderer
{
    private readonly double _speed;
    private readonly int _width;
    private readonly double _maxBrightness;

    public TravelingWaveRenderer(double speed, int width, double maxBrightness)
    {
        _speed = speed;
        _width = width;
        _maxBrightness = maxBrightness;
    }

    public void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        var position = (elapsedSeconds * _speed) % 8;
        
        for (int i = 0; i < 8; i++)
        {
            var distance = Math.Min(
                Math.Abs(i - position),
                8 - Math.Abs(i - position) // Wrap around
            );
            
            var brightness = distance < _width
                ? _maxBrightness * (1.0 - distance / _width)
                : 0.0;
                
            blinkt.SetPixel(i, color.R, color.G, color.B, brightness);
        }
        blinkt.Show();
    }
}

public class ScannerRenderer : IAnimationRenderer
{
    private readonly double _speed;
    private readonly double _maxBrightness;

    public ScannerRenderer(double speed, double maxBrightness)
    {
        _speed = speed;
        _maxBrightness = maxBrightness;
    }

    public void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        var cycle = (elapsedSeconds * _speed) % 2.0;
        var position = cycle < 1.0 ? cycle * 7 : (2.0 - cycle) * 7;
        
        for (int i = 0; i < 8; i++)
        {
            var distance = Math.Abs(i - position);
            var brightness = distance < 1.5
                ? _maxBrightness * (1.0 - distance / 1.5)
                : 0.0;
            blinkt.SetPixel(i, color.R, color.G, color.B, brightness);
        }
        blinkt.Show();
    }
}

public class FillRenderer : IAnimationRenderer
{
    private readonly double _speed;
    private readonly double _brightness;

    public FillRenderer(double speed, double brightness)
    {
        _speed = speed;
        _brightness = brightness;
    }

    public void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        var lit = (int)((elapsedSeconds * _speed) % 9);
        
        for (int i = 0; i < 8; i++)
        {
            var brightness = i < lit ? _brightness : 0.0;
            blinkt.SetPixel(i, color.R, color.G, color.B, brightness);
        }
        blinkt.Show();
    }
}

public class SparkleRenderer : IAnimationRenderer
{
    private readonly Random _random = new();
    private readonly double _brightness;
    private readonly double _sparsity;

    public SparkleRenderer(double brightness, double sparsity)
    {
        _brightness = brightness;
        _sparsity = sparsity;
    }

    public void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        for (int i = 0; i < 8; i++)
        {
            var brightness = _random.NextDouble() < _sparsity ? _brightness : 0.0;
            blinkt.SetPixel(i, color.R, color.G, color.B, brightness);
        }
        blinkt.Show();
    }
}

public class RainbowCycleRenderer : IAnimationRenderer
{
    private readonly double _speed;
    private readonly double _brightness;

    public RainbowCycleRenderer(double speed, double brightness)
    {
        _speed = speed;
        _brightness = brightness;
    }

    public void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        var offset = (elapsedSeconds * _speed) % 1.0;
        
        for (int i = 0; i < 8; i++)
        {
            var hue = ((i / 8.0 + offset) % 1.0) * 360;
            var rgb = HsvToRgb(hue, 1.0, 1.0);
            blinkt.SetPixel(i, rgb.R, rgb.G, rgb.B, _brightness);
        }
        blinkt.Show();
    }

    private static Color HsvToRgb(double h, double s, double v)
    {
        var c = v * s;
        var x = c * (1 - Math.Abs((h / 60) % 2 - 1));
        var m = v - c;

        double r = 0, g = 0, b = 0;
        if (h < 60) { r = c; g = x; }
        else if (h < 120) { r = x; g = c; }
        else if (h < 180) { g = c; b = x; }
        else if (h < 240) { g = x; b = c; }
        else if (h < 300) { r = x; b = c; }
        else { r = c; b = x; }

        return Color.FromArgb(
            (byte)((r + m) * 255),
            (byte)((g + m) * 255),
            (byte)((b + m) * 255)
        );
    }
}
