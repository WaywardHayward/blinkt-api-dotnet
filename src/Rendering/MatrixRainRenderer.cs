using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Matrix-style falling pixels with trails
/// </summary>
public class MatrixRainRenderer : IAnimationRenderer
{
    private readonly double _spawnChance;
    private readonly int _fallSpeed;
    private readonly int _trailLength;
    private readonly double _maxBrightness;
    private readonly Random _random = new();
    private readonly Dictionary<int, List<double>> _columns = new(); // column -> list of positions
    private const int PixelCount = 8;

    public MatrixRainRenderer(double spawnChance, int fallSpeed, int trailLength, double maxBrightness)
    {
        _spawnChance = spawnChance;
        _fallSpeed = fallSpeed;
        _trailLength = trailLength;
        _maxBrightness = maxBrightness;
        
        // Initialize columns (we only have 8 pixels, treat as one vertical column)
        _columns[0] = new List<double>();
    }

    public void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        // Spawn new drops
        if (_random.NextDouble() < _spawnChance)
        {
            _columns[0].Add(0);
        }

        controller.Clear();

        // Update and render drops
        var toRemove = new List<double>();
        foreach (var position in _columns[0].ToList())
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
            _columns[0].Remove(pos);
        }

        controller.Show();
    }
}
