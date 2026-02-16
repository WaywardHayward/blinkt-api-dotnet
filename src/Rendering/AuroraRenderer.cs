using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Aurora borealis effect with slow color shifts
/// </summary>
public class AuroraRenderer : AnimationRendererBase
{
    public override string TypeKey => "aurora";
    private readonly double _speed;
    private readonly double _brightnessVariation;
    private readonly double _maxBrightness;

    public AuroraRenderer(double speed, double brightnessVariation, double maxBrightness)
    {
        _speed = speed;
        _brightnessVariation = brightnessVariation;
        _maxBrightness = maxBrightness;
    }

    public static AuroraRenderer Create(JsonElement json)
 =>
        new(json.GetDouble("speed"), json.GetDouble("brightness_variation"), json.GetDouble("max_brightness"));

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        for (int i = 0; i < PixelCount; i++)
        {
            // Each pixel has its own wave phase
            var phase = (elapsedSeconds * _speed + i * 0.3) * Math.PI * 2;
            var wave = (Math.Sin(phase) + 1) / 2; // 0-1
            
            // Add secondary wave for complexity
            var phase2 = (elapsedSeconds * _speed * 0.7 - i * 0.2) * Math.PI * 2;
            var wave2 = (Math.Sin(phase2) + 1) / 2;
            
            var brightness = _maxBrightness * (wave * 0.6 + wave2 * 0.4);
            brightness += (_brightnessVariation * Math.Sin(elapsedSeconds + i));
            brightness = Math.Clamp(brightness, 0, _maxBrightness);
            
            // Shift towards green/cyan for aurora colors
            var r = (byte)Math.Clamp(color.R * 0.3, 0, 255);
            var g = (byte)Math.Clamp(color.G * 1.2, 0, 255);
            var b = (byte)Math.Clamp(color.B * 0.9, 0, 255);

            // Use RGB scaling for smooth brightness transitions
            var auroraColor = Color.FromArgb(r, g, b);
            SetPixelSmooth(controller, i, auroraColor, brightness);
        }

        controller.Show();
    }
}
