using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Organic fire with random ember pops
/// </summary>
public class OrganicFireRenderer : IAnimationRenderer
{
    private readonly double _baseBrightness;
    private readonly double _flickerAmount;
    private readonly double _emberChance;
    private readonly double _emberBrightness;
    private readonly Random _random = new();
    private const int PixelCount = 8;

    public OrganicFireRenderer(double baseBrightness, double flickerAmount, double emberChance, double emberBrightness)
    {
        _baseBrightness = baseBrightness;
        _flickerAmount = flickerAmount;
        _emberChance = emberChance;
        _emberBrightness = emberBrightness;
    }

    public void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        for (int i = 0; i < PixelCount; i++)
        {
            double brightness;
            
            // Random ember pop
            if (_random.NextDouble() < _emberChance)
            {
                brightness = _emberBrightness;
            }
            else
            {
                // Normal flicker
                var flicker = (_random.NextDouble() - 0.5) * 2 * _flickerAmount;
                brightness = Math.Max(0, _baseBrightness + flicker);
            }

            // Vary color warmth
            var colorShift = (_random.NextDouble() - 0.5) * 0.3;
            var r = (byte)Math.Clamp(color.R + (int)(colorShift * 60), 0, 255);
            var g = (byte)Math.Clamp(color.G + (int)(colorShift * 40), 0, 255);
            var b = (byte)Math.Clamp(color.B - (int)(colorShift * 30), 0, 255);

            controller.SetPixel(i, r, g, b, brightness);
        }

        controller.Show();
    }
}
