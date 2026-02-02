using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Fire flicker effect with random brightness variation and color warmth
/// </summary>
public class FireFlickerRenderer : IAnimationRenderer
{
    private readonly double _baseBrightness;
    private readonly double _flickerAmount;
    private readonly double _colorVariation;
    private readonly Random _random = new();
    private const int PixelCount = 8;

    public FireFlickerRenderer(double baseBrightness, double flickerAmount, double colorVariation)
    {
        _baseBrightness = baseBrightness;
        _flickerAmount = flickerAmount;
        _colorVariation = colorVariation;
    }

    public void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        for (int i = 0; i < PixelCount; i++)
        {
            // Random flicker for each pixel
            var flicker = (_random.NextDouble() - 0.5) * 2 * _flickerAmount;
            var brightness = Math.Max(0, _baseBrightness + flicker);

            // Vary color temperature (more red/orange variation)
            var colorShift = (_random.NextDouble() - 0.5) * _colorVariation;
            
            var r = (byte)Math.Clamp(color.R + (int)(colorShift * 50), 0, 255);
            var g = (byte)Math.Clamp(color.G + (int)(colorShift * 30), 0, 255);
            var b = (byte)Math.Clamp(color.B - (int)(colorShift * 20), 0, 255);

            controller.SetPixel(i, r, g, b, brightness);
        }

        controller.Show();
    }
}
