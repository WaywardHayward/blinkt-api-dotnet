using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Matrix-style falling pixels with trails
/// </summary>
public class MatrixRainRenderer : AnimationRendererBase
{
    public override string TypeKey => "matrix_rain";
    private readonly double _spawnChance;
    private readonly int _fallSpeed;
    private readonly int _trailLength;
    private readonly double _maxBrightness;
    private readonly List<double> _drops = new(); // spawn times of falling drops

    public MatrixRainRenderer(double spawnChance, int fallSpeed, int trailLength, double maxBrightness)
    {
        _spawnChance = spawnChance;
        _fallSpeed = fallSpeed;
        _trailLength = trailLength;
        _maxBrightness = maxBrightness;
    }

    public static MatrixRainRenderer Create(JsonElement json) =>
        new(json.GetDouble("spawn_chance"), json.GetInt("fall_speed"), json.GetInt("trail_length"), json.GetDouble("max_brightness"));

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        // Spawn new drops
        if (Random.NextDouble() < _spawnChance)
        {
            _drops.Add(elapsedSeconds); // store spawn time
        }

        controller.Clear();

        // Update and render drops
        var toRemove = new List<double>();
        foreach (var spawnTime in _drops.ToList())
        {
            var age = elapsedSeconds - spawnTime;
            var currentPos = age * _fallSpeed;
            
            if (currentPos >= PixelCount + _trailLength)
            {
                toRemove.Add(spawnTime);
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

        foreach (var spawnTime in toRemove)
        {
            _drops.Remove(spawnTime);
        }

        controller.Show();
    }
}
