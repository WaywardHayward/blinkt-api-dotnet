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
            _phases[i] = Random.NextDouble() * 2 * Math.PI;
            _speeds[i] = _minSpeed + Random.NextDouble() * (_maxSpeed - _minSpeed);
        }
    }

    public static PixelOscillatorRenderer Create(JsonElement json)
    {
        var (minBrightness, maxBrightness) = json.GetRange("brightness_range");
        var (minSpeed, maxSpeed) = json.GetRange("speed_range");
        return new(minBrightness, maxBrightness, minSpeed, maxSpeed);
    }

    public override void Render(BlinktController blinkt, Color color, double elapsedSeconds)
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
