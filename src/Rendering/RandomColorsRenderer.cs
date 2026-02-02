using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Random rapid color changes - disco party mode
/// </summary>
public class RandomColorsRenderer : IAnimationRenderer
{
    private readonly double _changeRate;
    private readonly double _brightness;
    private readonly Random _random = new();
    private double _lastChangeTime;
    private Color[] _currentColors = new Color[8];
    private const int PixelCount = 8;

    public RandomColorsRenderer(double changeRate, double brightness)
    {
        _changeRate = changeRate;
        _brightness = brightness;
        
        // Initialize with random colors
        for (int i = 0; i < PixelCount; i++)
        {
            _currentColors[i] = GetRandomColor();
        }
    }

    public void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        // Change colors at specified rate
        if (elapsedSeconds - _lastChangeTime > _changeRate)
        {
            for (int i = 0; i < PixelCount; i++)
            {
                _currentColors[i] = GetRandomColor();
            }
            _lastChangeTime = elapsedSeconds;
        }

        for (int i = 0; i < PixelCount; i++)
        {
            var c = _currentColors[i];
            controller.SetPixel(i, c.R, c.G, c.B, _brightness);
        }

        controller.Show();
    }

    private Color GetRandomColor()
    {
        var hue = _random.NextDouble() * 360;
        return HsvToRgb(hue, 1.0, 1.0);
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
