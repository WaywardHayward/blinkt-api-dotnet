using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Organic fire with random ember pops
/// </summary>
public class OrganicFireRenderer : AnimationRendererBase
{
    public override string TypeKey => "organic_fire";
    private readonly double _baseBrightness;
    private readonly double _flickerAmount;
    private readonly double _emberChance;
    private readonly double _emberBrightness;

    public OrganicFireRenderer(double baseBrightness, double flickerAmount, double emberChance, double emberBrightness)
    {
        _baseBrightness = baseBrightness;
        _flickerAmount = flickerAmount;
        _emberChance = emberChance;
        _emberBrightness = emberBrightness;
    }

    public static OrganicFireRenderer Create(JsonElement json)
 =>
        new(json.GetDouble("base_brightness"), json.GetDouble("flicker_amount"), json.GetDouble("ember_chance"), json.GetDouble("ember_brightness"));

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        ForEachPixel(controller, (ctrl, i) =>
        {
            var brightness = CalculatePixelBrightness();
            var (r, g, b) = CalculateColorWithWarmth(color);
            ctrl.SetPixel(i, r, g, b, brightness);
        });
        
        controller.Show();
    }

    private double CalculatePixelBrightness()
    {
        if (IsEmber())
            return _emberBrightness;

        return CalculateFlickerBrightness();
    }

    private bool IsEmber() => Random.NextDouble() < _emberChance;

    private double CalculateFlickerBrightness()
    {
        var flicker = (Random.NextDouble() - 0.5) * 2 * _flickerAmount;
        return Math.Max(0, _baseBrightness + flicker);
    }

    private (byte r, byte g, byte b) CalculateColorWithWarmth(Color baseColor)
    {
        var colorShift = (Random.NextDouble() - 0.5) * 0.3;
        
        var r = (byte)Math.Clamp(baseColor.R + (int)(colorShift * 60), 0, 255);
        var g = (byte)Math.Clamp(baseColor.G + (int)(colorShift * 40), 0, 255);
        var b = (byte)Math.Clamp(baseColor.B - (int)(colorShift * 30), 0, 255);

        return (r, g, b);
    }
}
