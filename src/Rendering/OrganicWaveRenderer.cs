using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Organic wave effect with speed variation for natural motion
/// </summary>
public class OrganicWaveRenderer : AnimationRendererBase
{
    public override string TypeKey => "organic_wave";
    private readonly double _baseSpeed;
    private readonly double _speedVariation;
    private readonly int _width;
    private readonly double _maxBrightness;
    private double _currentSpeed;
    private double _speedChangeTime;

    public OrganicWaveRenderer(double baseSpeed, double speedVariation, int width, double maxBrightness)
    {
        _baseSpeed = baseSpeed;
        _speedVariation = speedVariation;
        _width = width;
        _maxBrightness = maxBrightness;
        _currentSpeed = baseSpeed;
    }

    public static OrganicWaveRenderer Create(JsonElement json)
 =>
        new(json.GetDouble("base_speed"), json.GetDouble("speed_variation"), json.GetInt("width"), json.GetDouble("max_brightness"));

    public override void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        // Smoothly vary speed for organic feel
        if (elapsedSeconds > _speedChangeTime)
        {
            _currentSpeed = _baseSpeed + (Random.NextDouble() - 0.5) * 2 * _speedVariation;
            _speedChangeTime = elapsedSeconds + 2.0; // Change every 2 seconds
        }

        var position = (elapsedSeconds * _currentSpeed) % PixelCount;
        
        for (int i = 0; i < PixelCount; i++)
        {
            var distance = Math.Min(
                Math.Abs(i - position),
                PixelCount - Math.Abs(i - position)
            );
            
            var brightness = distance < _width
                ? _maxBrightness * (1.0 - distance / _width)
                : 0.0;
                
            blinkt.SetPixel(i, color.R, color.G, color.B, brightness);
        }
        blinkt.Show();
    }
}
