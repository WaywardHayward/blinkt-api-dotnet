using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Aurora borealis effect with slow color shifts
/// </summary>
public class AuroraRenderer : IAnimationRenderer
{
    private readonly double _speed;
    private readonly double _brightnessVariation;
    private readonly double _maxBrightness;
    private const int PixelCount = 8;

    public AuroraRenderer(double speed, double brightnessVariation, double maxBrightness)
    {
        _speed = speed;
        _brightnessVariation = brightnessVariation;
        _maxBrightness = maxBrightness;
    }

    public void Render(BlinktController controller, Color color, double elapsedSeconds)
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
            var r = (byte)(color.R * 0.3);
            var g = (byte)(color.G * 1.2);
            var b = (byte)(color.B * 0.9);

            controller.SetPixel(i, r, g, b, brightness);
        }

        controller.Show();
    }
}
