using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

public class SparkleRenderer : AnimationRendererBase
{
    public override string TypeKey => "sparkle";
    private readonly double _brightness;
    private readonly double _sparsity;
    private Color _currentColor;

    public SparkleRenderer(double brightness, double sparsity)
    {
        _brightness = brightness;
        _sparsity = sparsity;
    }

    public static SparkleRenderer Create(JsonElement json) =>
        new(json.GetDouble("brightness"), json.GetDouble("sparsity"));

    public override void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        _currentColor = color;
        ForEachPixel(blinkt, RenderPixel);
        blinkt.Show();
    }

    private void RenderPixel(BlinktController ctrl, int i)
    {
        var brightness = Random.NextDouble() < _sparsity ? _brightness : 0.0;
        // Use RGB scaling for smooth brightness (on/off but consistent with other renderers)
        SetPixelSmooth(ctrl, i, _currentColor, brightness);
    }
}
