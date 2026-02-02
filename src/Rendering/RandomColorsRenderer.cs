using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Random rapid color changes - disco party mode
/// </summary>
public class RandomColorsRenderer : AnimationRendererBase
{
    private readonly double _changeRate;
    private readonly double _brightness;
    private double _lastChangeTime;
    private Color[] _currentColors = new Color[PixelCount];

    public RandomColorsRenderer(double changeRate, double brightness)
    {
        _changeRate = changeRate;
        _brightness = brightness;
        
        // Initialize with random colors
        for (int i = 0; i < PixelCount; i++)
        {
            _currentColors[i] = GetRandomColor();
        }
    }

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        // Change colors at specified rate
        if (elapsedSeconds - _lastChangeTime > _changeRate)
        {
            for (int i = 0; i < PixelCount; i++)
            {
                _currentColors[i] = GetRandomColor();
            }
            _lastChangeTime = elapsedSeconds;
        }

        for (int i = 0; i < PixelCount; i++)
        {
            var c = _currentColors[i];
            controller.SetPixel(i, c.R, c.G, c.B, _brightness);
        }

        controller.Show();
    }

    private Color GetRandomColor()
    {
        var hue = Random.NextDouble() * 360;
        return HsvToRgb(hue, 1.0, 1.0);
    }
}
