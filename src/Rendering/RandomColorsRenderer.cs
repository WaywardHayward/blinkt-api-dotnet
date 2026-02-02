using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Random rapid color changes - disco party mode
/// </summary>
public class RandomColorsRenderer : AnimationRendererBase
{
    public override string TypeKey => "random_colors";
    private readonly double _changeRate;
    private readonly double _brightness;
    private double _lastChangeTime;
    private Color[] _currentColors = new Color[PixelCount];

    public RandomColorsRenderer(double changeRate, double brightness)
    {
        _changeRate = changeRate;
        _brightness = brightness;
        InitializeRandomColors();
    }

    public static RandomColorsRenderer Create(JsonElement json) =>
        new(json.GetDouble("change_rate"), json.GetDouble("brightness"));

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        if (ShouldChangeColors(elapsedSeconds))
        {
            UpdateAllColors();
            _lastChangeTime = elapsedSeconds;
        }

        RenderCurrentColors(controller);
        controller.Show();
    }

    private void InitializeRandomColors()
    {
        ForEachPixel(i => _currentColors[i] = GetRandomColor());
    }

    private bool ShouldChangeColors(double elapsedSeconds) =>
        elapsedSeconds - _lastChangeTime > _changeRate;

    private void UpdateAllColors()
    {
        ForEachPixel(i => _currentColors[i] = GetRandomColor());
    }

    private void RenderCurrentColors(BlinktController controller)
    {
        ForEachPixel(controller, (ctrl, i) =>
        {
            var c = _currentColors[i];
            ctrl.SetPixel(i, c.R, c.G, c.B, _brightness);
        });
    }

    private Color GetRandomColor()
    {
        var hue = Random.NextDouble() * 360;
        return HsvToRgb(hue, 1.0, 1.0);
    }
}
