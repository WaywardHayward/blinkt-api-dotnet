using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Meditation breathing animation - soft magenta/purple with slow sine wave.
/// Uses RGB scaling for ultra-smooth brightness transitions.
/// </summary>
public class BreatheRenderer : AnimationRendererBase
{
    public override string TypeKey => "breathe";
    
    private readonly double _periodSeconds;
    private readonly double _minBrightness;
    private readonly double _maxBrightness;
    private readonly Color _breatheColor;

    public BreatheRenderer(double periodSeconds, double minBrightness, double maxBrightness, Color color)
    {
        _periodSeconds = periodSeconds;
        _minBrightness = minBrightness;
        _maxBrightness = maxBrightness;
        _breatheColor = color;
    }

    public static BreatheRenderer Create(JsonElement json)
    {
        var periodSeconds = json.TryGetProperty("period_seconds", out var period) 
            ? period.GetDouble() 
            : 5.0;
        
        var minBrightness = json.TryGetProperty("min_brightness", out var minB) 
            ? minB.GetDouble() 
            : 0.3;
        
        var maxBrightness = json.TryGetProperty("max_brightness", out var maxB) 
            ? maxB.GetDouble() 
            : 1.0;
        
        // Default to magenta (#FF00FF)
        byte r = 255, g = 0, b = 255;
        if (json.TryGetProperty("color", out var colorProp))
        {
            var colorStr = colorProp.GetString();
            if (!string.IsNullOrEmpty(colorStr) && colorStr.StartsWith("#") && colorStr.Length == 7)
            {
                r = Convert.ToByte(colorStr.Substring(1, 2), 16);
                g = Convert.ToByte(colorStr.Substring(3, 2), 16);
                b = Convert.ToByte(colorStr.Substring(5, 2), 16);
            }
        }
        
        return new BreatheRenderer(periodSeconds, minBrightness, maxBrightness, Color.FromArgb(r, g, b));
    }

    public override void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        // Slow sine wave - one complete breath cycle over periodSeconds
        var phase = (elapsedSeconds / _periodSeconds) * 2 * Math.PI;
        
        // Sine wave: smooth 0 to 1 to 0
        var sineValue = (Math.Sin(phase - Math.PI / 2) + 1) / 2;
        
        // Map to brightness range (never fully off for gentle effect)
        var brightness = _minBrightness + (sineValue * (_maxBrightness - _minBrightness));
        
        // Use RGB scaling for buttery smooth transitions (256 levels vs 32 hardware levels)
        SetAllSmooth(blinkt, _breatheColor, brightness);
        blinkt.Show();
    }
}
