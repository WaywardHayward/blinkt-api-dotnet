using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Slow hue rotation through the full color spectrum.
/// All 8 LEDs display the same color, drifting peacefully through the rainbow.
/// </summary>
public class SpectrumRenderer : AnimationRendererBase
{
    public override string TypeKey => "spectrum";
    
    private readonly double _cycleDurationSeconds;
    private readonly double _brightness;
    private readonly double _saturation;

    public SpectrumRenderer(double cycleDurationSeconds, double brightness, double saturation)
    {
        _cycleDurationSeconds = cycleDurationSeconds;
        _brightness = brightness;
        _saturation = saturation;
    }

    public static SpectrumRenderer Create(JsonElement json)
    {
        var cycleDuration = json.TryGetProperty("cycle_duration_seconds", out var cycle) 
            ? cycle.GetDouble() 
            : 45.0; // Default 45 seconds for full spectrum
        
        var brightness = json.TryGetProperty("brightness", out var bright) 
            ? bright.GetDouble() 
            : 0.5;
        
        var saturation = json.TryGetProperty("saturation", out var sat) 
            ? sat.GetDouble() 
            : 1.0;
        
        return new SpectrumRenderer(cycleDuration, brightness, saturation);
    }

    public override void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        // Slow rotation through 360 degrees of hue
        var hue = (elapsedSeconds / _cycleDurationSeconds) * 360.0;
        hue = hue % 360.0; // Keep in 0-360 range
        
        // Convert HSV to RGB - constant brightness, just shifting hue
        var rgb = HsvToRgb(hue, _saturation, 1.0);
        
        // Apply brightness via RGB scaling for smooth appearance
        SetAllSmooth(blinkt, rgb, _brightness);
        blinkt.Show();
    }
}
