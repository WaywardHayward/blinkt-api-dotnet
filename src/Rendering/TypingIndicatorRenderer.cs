using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Three dots bouncing like a typing indicator
/// </summary>
public class TypingIndicatorRenderer : IAnimationRenderer
{
    private readonly double _bounceSpeed;
    private readonly int _dotSpacing;
    private readonly double _brightness;
    private const int PixelCount = 8;

    public TypingIndicatorRenderer(double bounceSpeed, int dotSpacing, double brightness)
    {
        _bounceSpeed = bounceSpeed;
        _dotSpacing = dotSpacing;
        _brightness = brightness;
    }

    public void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        controller.Clear();

        // Three dots bounce up and down sequentially
        for (int dot = 0; dot < 3; dot++)
        {
            var phase = (elapsedSeconds * _bounceSpeed + dot * 0.2) * Math.PI * 2;
            var bounce = (Math.Sin(phase) + 1) / 2; // 0-1
            
            // Map bounce to brightness (0.5-1.0 range for subtle effect)
            var dotBrightness = _brightness * (0.5 + bounce * 0.5);
            
            var pixelIndex = 2 + (dot * _dotSpacing); // Start at pixel 2, space dots out
            if (pixelIndex < PixelCount)
            {
                controller.SetPixel(pixelIndex, color.R, color.G, color.B, dotBrightness);
            }
        }

        controller.Show();
    }
}
