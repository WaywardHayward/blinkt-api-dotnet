using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

public class SparkleRenderer : IAnimationRenderer
{
    private readonly Random _random = new();
    private readonly double _brightness;
    private readonly double _sparsity;

    public SparkleRenderer(double brightness, double sparsity)
    {
        _brightness = brightness;
        _sparsity = sparsity;
    }

    public void Render(BlinktController blinkt, Color color, double elapsedSeconds)
    {
        for (int i = 0; i < 8; i++)
        {
            var brightness = _random.NextDouble() < _sparsity ? _brightness : 0.0;
            blinkt.SetPixel(i, color.R, color.G, color.B, brightness);
        }
        blinkt.Show();
    }
}
