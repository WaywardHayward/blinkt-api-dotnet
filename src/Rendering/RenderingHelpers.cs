using System.Drawing;

namespace BlinktApi.Rendering;

/// <summary>
/// Helper utilities for animation renderers
/// </summary>
public static class RenderingHelpers
{
    public const int PixelCount = 8;

    /// <summary>
    /// Convert HSV color to RGB
    /// </summary>
    public static Color HsvToRgb(double h, double s, double v)
    {
        // Normalize hue to [0, 360)
        h = ((h % 360) + 360) % 360;
        
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

    /// <summary>
    /// Thread-safe random number generator
    /// </summary>
    private static readonly ThreadLocal<Random> ThreadRandom = new(() => new Random());
    
    public static Random Random => ThreadRandom.Value!;
}
