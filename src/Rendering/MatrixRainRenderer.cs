using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Matrix-style falling pixels with trails
/// </summary>
public class MatrixRainRenderer : AnimationRendererBase
{
    private readonly double _spawnChance;
    private readonly int _fallSpeed;
    private readonly int _trailLength;
    private readonly double _maxBrightness;
    private readonly List<double> _drops = new(); // positions of falling drops

    public MatrixRainRenderer(double spawnChance, int fallSpeed, int trailLength, double maxBrightness)
    {
        _spawnChance = spawnChance;
        _fallSpeed = fallSpeed;
        _trailLength = trailLength;
        _maxBrightness = maxBrightness;
    }

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        // Spawn new drops
        if (Random.NextDouble() < _spawnChance)
        {
            _drops.Add(0);
        }

        controller.Clear();

        // Update and render drops
        var toRemove = new List<double>();
        foreach (var position in _drops.ToList())
        {
            var currentPos = position + (elapsedSeconds * _fallSpeed);
            
            if (currentPos >= PixelCount + _trailLength)
            {
                toRemove.Add(position);
                continue;
            }

            // Draw trail
            for (int i = 0; i < _trailLength; i++)
            {
                var pixel = (int)(currentPos - i);
                if (pixel >= 0 && pixel < PixelCount)
                {
                    var trailBrightness = _maxBrightness * (1.0 - (double)i / _trailLength);
                    controller.SetPixel(pixel, color.R, color.G, color.B, trailBrightness);
                }
            }
        }

        foreach (var pos in toRemove)
        {
            _drops.Remove(pos);
        }

        controller.Show();
    }
}
