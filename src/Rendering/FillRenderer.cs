using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

public class FillRenderer : AnimationRendererBase
{
    public override string TypeKey => "sequential_fill";
    private readonly double _speed;
    private readonly double _brightness;
    private Color _currentColor;
    private int _litPixels;

    public FillRenderer(double speed, double brightness)
    {
        _speed = speed;
        _brightness = brightness;
    }

    public static FillRenderer Create(JsonElement json) =>
        new(json.GetDouble("fps"), json.GetDouble("brightness"));

    public override void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        _currentColor = color;
        _litPixels = (int)((elapsedSeconds * _speed) % 9);
        
        ForEachPixel(blinkt, RenderPixel);
        blinkt.Show();
    }

    private void RenderPixel(BlinktController ctrl, int i)
    {
        var brightness = i < _litPixels ? _brightness : 0.0;
        ctrl.SetPixel(i, _currentColor.R, _currentColor.G, _currentColor.B, brightness);
    }
}
