using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

public class ScannerRenderer : AnimationRendererBase
{
    public override string TypeKey => "scanner";
    
    private readonly double _speed;
    private readonly double _maxBrightness;
    private Color _currentColor;
    private double _position;

    public ScannerRenderer(double speed, double maxBrightness)
    {
        _speed = speed;
        _maxBrightness = maxBrightness;
    }

    public static ScannerRenderer Create(JsonElement json) =>
        new(json.GetDouble("speed"), json.GetDouble("max_brightness"));

    public override void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        _currentColor = color;
        var cycle = (elapsedSeconds * _speed) % 2.0;
        _position = cycle < 1.0 ? cycle * 7 : (2.0 - cycle) * 7;
        
        ForEachPixel(blinkt, RenderPixel);
        blinkt.Show();
    }

    private void RenderPixel(BlinktController ctrl, int i)
    {
        var distance = Math.Abs(i - _position);
        var brightness = distance < 1.5
            ? _maxBrightness * (1.0 - distance / 1.5)
            : 0.0;
        ctrl.SetPixel(i, _currentColor.R, _currentColor.G, _currentColor.B, brightness);
    }
}
