using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Rainbow color cycle across pixels
/// </summary>
public class ColorCycleRenderer : IAnimationRenderer
{
    private readonly double _rotationSpeed;
    private readonly double _brightness;
    private readonly double _spacing;
    private const int PixelCount = 8;

    public ColorCycleRenderer(double rotationSpeed, double brightness, double spacing)
    {
        _rotationSpeed = rotationSpeed;
        _brightness = brightness;
        _spacing = spacing;
    }

    public void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        var rotation = elapsedSeconds * _rotationSpeed * 360;
        
        for (int i = 0; i < PixelCount; i++)
        {
            // Calculate hue for this pixel
            var hue = (rotation + i * _spacing) % 360;
            var rgb = HsvToRgb(hue, 1.0, 1.0);
            
            controller.SetPixel(i, rgb.R, rgb.G, rgb.B, _brightness);
        }

        controller.Show();
    }

    private static Color HsvToRgb(double h, double s, double v)
    {
        int hi = (int)(h / 60) % 6;
        double f = h / 60 - Math.Floor(h / 60);

        double p = v * (1 - s);
        double q = v * (1 - f * s);
        double t = v * (1 - (1 - f) * s);

        return hi switch
        {
            0 => Color.FromArgb((int)(v * 255), (int)(t * 255), (int)(p * 255)),
            1 => Color.FromArgb((int)(q * 255), (int)(v * 255), (int)(p * 255)),
            2 => Color.FromArgb((int)(p * 255), (int)(v * 255), (int)(t * 255)),
            3 => Color.FromArgb((int)(p * 255), (int)(q * 255), (int)(v * 255)),
            4 => Color.FromArgb((int)(t * 255), (int)(p * 255), (int)(v * 255)),
            _ => Color.FromArgb((int)(v * 255), (int)(p * 255), (int)(q * 255))
        };
    }
}
