using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

public class FillRenderer : AnimationRendererBase
{
    public override string TypeKey => "fill";
    private readonly double _speed;
    private readonly double _brightness;

    public FillRenderer(double speed, double brightness)
    {
        _speed = speed;
        _brightness = brightness;
    }

    public static FillRenderer Create(JsonElement json) =>
        new(json.GetDouble("speed"), json.GetDouble("brightness"));

    public override void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        var lit = (int)((elapsedSeconds * _speed) % 9);
        
        for (int i = 0; i < 8; i++)
        {
            var brightness = i < lit ? _brightness : 0.0;
            blinkt.SetPixel(i, color.R, color.G, color.B, brightness);
        }
        blinkt.Show();
    }
}
