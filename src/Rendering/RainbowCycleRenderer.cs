using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

public class RainbowCycleRenderer : AnimationRendererBase
{
    private readonly double _speed;
    private readonly double _brightness;

    public RainbowCycleRenderer(double speed, double brightness)
    {
        _speed = speed;
        _brightness = brightness;
    }

    public override void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        var offset = (elapsedSeconds * _speed) % 1.0;
        
        for (int i = 0; i < PixelCount; i++)
        {
            var hue = ((i / (double)PixelCount + offset) % 1.0) * 360;
            var rgb = HsvToRgb(hue, 1.0, 1.0);
            blinkt.SetPixel(i, rgb.R, rgb.G, rgb.B, _brightness);
        }
        blinkt.Show();
    }
}
