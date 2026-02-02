using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

public class TravelingWaveRenderer : AnimationRendererBase
{
    public override string TypeKey => "traveling_wave";
    private readonly double _speed;
    private readonly int _width;
    private readonly double _maxBrightness;
    private Color _currentColor;
    private double _position;

    public TravelingWaveRenderer(double speed, int width, double maxBrightness)
    {
        _speed = speed;
        _width = width;
        _maxBrightness = maxBrightness;
    }

    public static TravelingWaveRenderer Create(JsonElement json) =>
        new(json.GetDouble("speed"), json.GetInt("width"), json.GetDouble("max_brightness"));

    public override void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        _currentColor = color;
        _position = (elapsedSeconds * _speed) % 8;
        
        ForEachPixel(blinkt, RenderPixel);
        blinkt.Show();
    }

    private void RenderPixel(BlinktController ctrl, int i)
    {
        var distance = Math.Min(
            Math.Abs(i - _position),
            8 - Math.Abs(i - _position) // Wrap around
        );
        
        var brightness = distance < _width
            ? _maxBrightness * (1.0 - distance / _width)
            : 0.0;
            
        ctrl.SetPixel(i, _currentColor.R, _currentColor.G, _currentColor.B, brightness);
    }
}
