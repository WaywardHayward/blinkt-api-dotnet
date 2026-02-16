using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

public class PixelOscillatorRenderer : AnimationRendererBase
{
    public override string TypeKey => "pixel_oscillator";
    private readonly double _minBrightness;
    private readonly double _maxBrightness;
    private readonly double _minSpeed;
    private readonly double _maxSpeed;
    private readonly double[] _phases;
    private readonly double[] _speeds;
    private Color _currentColor;

    public PixelOscillatorRenderer(double minBrightness, double maxBrightness, double minSpeed, double maxSpeed)
    {
        _minBrightness = minBrightness;
        _maxBrightness = maxBrightness;
        _minSpeed = minSpeed;
        _maxSpeed = maxSpeed;
        
        _phases = new double[PixelCount];
        _speeds = new double[PixelCount];
        InitializePhases();
    }

    public static PixelOscillatorRenderer Create(JsonElement json)
    {
        var (minBrightness, maxBrightness) = json.GetRange("brightness_range");
        var (minSpeed, maxSpeed) = json.GetRange("speed_range");
        return new(minBrightness, maxBrightness, minSpeed, maxSpeed);
    }

    public override void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        _currentColor = color;
        ForEachPixel(blinkt, RenderPixel);
        blinkt.Show();
    }

    private void InitializePhases()
    {
        ForEachPixel(InitializePixelPhase);
    }

    private void InitializePixelPhase(int i)
    {
        _phases[i] = Random.NextDouble() * 2 * Math.PI;
        _speeds[i] = _minSpeed + Random.NextDouble() * (_maxSpeed - _minSpeed);
    }

    private void RenderPixel(BlinktController ctrl, int i)
    {
        _phases[i] += _speeds[i];
        var brightness = (Math.Sin(_phases[i]) + 1) / 2;
        brightness = _minBrightness + (brightness * (_maxBrightness - _minBrightness));
        
        // Use RGB scaling for smooth brightness transitions
        SetPixelSmooth(ctrl, i, _currentColor, brightness);
    }
}
