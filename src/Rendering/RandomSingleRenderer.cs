using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Random single pixel sparkle effect
/// </summary>
public class RandomSingleRenderer : IAnimationRenderer
{
    private readonly double _brightness;
    private readonly Random _random = new();
    private const int PixelCount = 8;

    public RandomSingleRenderer(double brightness)
    {
        _brightness = brightness;
    }

    public void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        // Clear all
        controller.Clear();
        
        // Light up one random pixel
        var pixel = _random.Next(PixelCount);
        controller.SetPixel(pixel, color.R, color.G, color.B, _brightness);

        controller.Show();
    }
}
