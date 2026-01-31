using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

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
