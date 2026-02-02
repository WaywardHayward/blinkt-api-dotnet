using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

public class TravelingWaveRenderer : AnimationRendererBase
{
    private readonly double _speed;
    private readonly int _width;
    private readonly double _maxBrightness;

    public TravelingWaveRenderer(double speed, int width, double maxBrightness)
    {
        _speed = speed;
        _width = width;
        _maxBrightness = maxBrightness;
    }

    public override void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        var position = (elapsedSeconds * _speed) % 8;
        
        for (int i = 0; i < 8; i++)
        {
            var distance = Math.Min(
                Math.Abs(i - position),
                8 - Math.Abs(i - position) // Wrap around
            );
            
            var brightness = distance < _width
                ? _maxBrightness * (1.0 - distance / _width)
                : 0.0;
                
            blinkt.SetPixel(i, color.R, color.G, color.B, brightness);
        }
        blinkt.Show();
    }
}
