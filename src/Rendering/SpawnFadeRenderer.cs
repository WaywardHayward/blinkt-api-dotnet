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
        // Spawn new pixels randomly
        for (int i = 0; i < PixelCount; i++)
        {
            if (!_activePixels.ContainsKey(i) && Random.NextDouble() < _spawnChance)
            {
                _activePixels[i] = 0;
            }
        }

        controller.Clear();

        // Update and render active pixels
        var pixelsToRemove = new List<int>();
        foreach (var kvp in _activePixels)
        {
            var pixel = kvp.Key;
            var age = kvp.Value;

            if (age >= _fadeFrames)
            {
                pixelsToRemove.Add(pixel);
                continue;
            }

            // Fade brightness over lifetime
            var lifetimeProgress = (double)age / _fadeFrames;
            var brightness = _maxBrightness * (1.0 - lifetimeProgress);

            controller.SetPixel(pixel, color.R, color.G, color.B, brightness);
            
            _activePixels[pixel] = age + 1;
        }

        // Clean up dead pixels
        foreach (var pixel in pixelsToRemove)
        {
            _activePixels.Remove(pixel);
        }

        controller.Show();
    }
}
