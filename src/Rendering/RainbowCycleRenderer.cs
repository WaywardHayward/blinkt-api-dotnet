using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

public class RainbowCycleRenderer : IAnimationRenderer
{
    private readonly double _speed;
    private readonly double _brightness;

    public RainbowCycleRenderer(double speed, double brightness)
    {
        _speed = speed;
        _brightness = brightness;
    }

    public void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        var offset = (elapsedSeconds * _speed) % 1.0;
        
        for (int i = 0; i < 8; i++)
        {
            var hue = ((i / 8.0 + offset) % 1.0) * 360;
            var rgb = HsvToRgb(hue, 1.0, 1.0);
            blinkt.SetPixel(i, rgb.R, rgb.G, rgb.B, _brightness);
        }
        blinkt.Show();
    }

    private static Color HsvToRgb(double h, double s, double v)
    {
        var c = v * s;
        var x = c * (1 - Math.Abs((h / 60) % 2 - 1));
        var m = v - c;

        double r = 0, g = 0, b = 0;
        if (h < 60) { r = c; g = x; }
        else if (h < 120) { r = x; g = c; }
        else if (h < 180) { g = c; b = x; }
        else if (h < 240) { g = x; b = c; }
        else if (h < 300) { r = x; b = c; }
        else { r = c; b = x; }

        return Color.FromArgb(
            (byte)((r + m) * 255),
            (byte)((g + m) * 255),
            (byte)((b + m) * 255)
        );
    }
}
