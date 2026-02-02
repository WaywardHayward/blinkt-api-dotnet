using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

public class ScannerRenderer : AnimationRendererBase
{
    private readonly double _speed;
    private readonly double _maxBrightness;

    public ScannerRenderer(double speed, double maxBrightness)
    {
        _speed = speed;
        _maxBrightness = maxBrightness;
    }

    public override void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        var cycle = (elapsedSeconds * _speed) % 2.0;
        var position = cycle < 1.0 ? cycle * 7 : (2.0 - cycle) * 7;
        
        for (int i = 0; i < 8; i++)
        {
            var distance = Math.Abs(i - position);
            var brightness = distance < 1.5
                ? _maxBrightness * (1.0 - distance / 1.5)
                : 0.0;
            blinkt.SetPixel(i, color.R, color.G, color.B, brightness);
        }
        blinkt.Show();
    }
}
