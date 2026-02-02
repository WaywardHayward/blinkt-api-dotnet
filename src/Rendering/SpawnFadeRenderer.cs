using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Spawn and fade effect - pixels randomly light up and fade out (like rain, snow, sparkles)
/// </summary>
public class SpawnFadeRenderer : AnimationRendererBase
{
    private readonly double _spawnChance;
    private readonly int _fadeFrames;
    private readonly double _maxBrightness;
    private readonly Dictionary<int, int> _activePixels = new(); // pixel -> frames alive

    public SpawnFadeRenderer(double spawnChance, int fadeFrames, double maxBrightness)
    {
        _spawnChance = spawnChance;
        _fadeFrames = fadeFrames;
        _maxBrightness = maxBrightness;
    }

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        SpawnNewPixels();
        controller.Clear();
        UpdateAndRenderActivePixels(controller, color);
        RemoveDeadPixels();
        controller.Show();
    }

    private void SpawnNewPixels()
    {
        ForEachPixel(i =>
        {
            if (ShouldSpawnPixel(i))
                _activePixels[i] = 0;
        });
    }

    private bool ShouldSpawnPixel(int pixel) =>
        !_activePixels.ContainsKey(pixel) && Random.NextDouble() < _spawnChance;

    private void UpdateAndRenderActivePixels(BlinktController controller, Color color)
    {
        foreach (var kvp in _activePixels)
        {
            var pixel = kvp.Key;
            var age = kvp.Value;

            if (age >= _fadeFrames)
                continue;

            var brightness = CalculateFadedBrightness(age);
            controller.SetPixel(pixel, color.R, color.G, color.B, brightness);
            _activePixels[pixel] = age + 1;
        }
    }

    private double CalculateFadedBrightness(int age)
    {
        var lifetimeProgress = (double)age / _fadeFrames;
        return _maxBrightness * (1.0 - lifetimeProgress);
    }

    private void RemoveDeadPixels()
    {
        var pixelsToRemove = _activePixels
            .Where(kvp => kvp.Value >= _fadeFrames)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var pixel in pixelsToRemove)
        {
            _activePixels.Remove(pixel);
        }
    }
}
