using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

public class SparkleRenderer : AnimationRendererBase
{
    public override string TypeKey => "sparkle";
    private readonly double _brightness;
    private readonly double _sparsity;

    public SparkleRenderer(double brightness, double sparsity)
    {
        _brightness = brightness;
        _sparsity = sparsity;
    }

    public static SparkleRenderer Create(JsonElement json)
 =>
        new(json.GetDouble("brightness"), json.GetDouble("sparsity"));

    public override void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        ForEachPixel(blinkt, (ctrl, i) =>
        {
            var brightness = Random.NextDouble() < _sparsity ? _brightness : 0.0;
            ctrl.SetPixel(i, color.R, color.G, color.B, brightness);
        });
        blinkt.Show();
    }
}
