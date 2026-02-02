using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Heartbeat-style pulse emanating from center
/// </summary>
public class CenterPulseRenderer : AnimationRendererBase
{
    public override string TypeKey => "center_pulse";
    private readonly double _pulseSpeed;
    private readonly double _maxBrightness;
    private readonly double _beatInterval;

    public CenterPulseRenderer(double pulseSpeed, double maxBrightness, double beatInterval)
    {
        if (beatInterval <= 0)
            throw new ArgumentOutOfRangeException(nameof(beatInterval), 
                "Beat interval must be positive to avoid division by zero in heartbeat timing.");
        
        _pulseSpeed = pulseSpeed;
        _maxBrightness = maxBrightness;
        _beatInterval = beatInterval;
    }

    public static CenterPulseRenderer Create(JsonElement json)
 =>
        new(json.GetDouble("pulse_speed"), json.GetDouble("max_brightness"), json.GetDouble("beat_interval"));

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        // Heartbeat: two pulses per beat interval
        var beatPhase = (elapsedSeconds % _beatInterval) / _beatInterval;
        
        // Two beats: 0-0.15 (first beat), 0.3-0.45 (second beat)
        double pulseIntensity = 0;
        if (beatPhase < 0.15)
        {
            // First beat
            var pulse = beatPhase / 0.15;
            pulseIntensity = Math.Sin(pulse * Math.PI) * _maxBrightness;
        }
        else if (beatPhase > 0.3 && beatPhase < 0.45)
        {
            // Second beat (slightly weaker)
            var pulse = (beatPhase - 0.3) / 0.15;
            pulseIntensity = Math.Sin(pulse * Math.PI) * _maxBrightness * 0.7;
        }

        // Apply to all pixels with falloff from center
        var center = (PixelCount - 1) / 2.0;
        for (int i = 0; i < PixelCount; i++)
        {
            var distanceFromCenter = Math.Abs(i - center) / center;
            var brightness = pulseIntensity * (1.0 - distanceFromCenter * 0.5);
            
            controller.SetPixel(i, color.R, color.G, color.B, Math.Max(0, brightness));
        }

        controller.Show();
    }
}
