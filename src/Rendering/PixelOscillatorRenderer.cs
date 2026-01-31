using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

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
