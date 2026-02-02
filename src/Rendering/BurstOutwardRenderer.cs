using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Burst expanding outward from center like an explosion
/// </summary>
public class BurstOutwardRenderer : AnimationRendererBase
{
    private readonly double _burstSpeed;
    private readonly double _fadeSpeed;
    private readonly double _maxBrightness;

    public BurstOutwardRenderer(double burstSpeed, double fadeSpeed, double maxBrightness)
    {
        _burstSpeed = burstSpeed;
        _fadeSpeed = fadeSpeed;
        _maxBrightness = maxBrightness;
    }

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        // Wave expands from center outward
        // For even pixel count, center between the two middle LEDs for symmetry
        var center = (PixelCount - 1) / 2.0;
        var wavePosition = elapsedSeconds * _burstSpeed * PixelCount;
        
        for (int i = 0; i < PixelCount; i++)
        {
            var distanceFromCenter = Math.Abs(i - center);
            var distanceFromWave = Math.Abs(wavePosition - distanceFromCenter);
            
            // Brightness peaks at wave position and fades with distance
            var brightness = Math.Max(0, _maxBrightness * (1.0 - distanceFromWave * _fadeSpeed));
            
            controller.SetPixel(i, color.R, color.G, color.B, brightness);
        }

        controller.Show();
    }
}
