using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Random single pixel sparkle effect
/// </summary>
public class RandomSingleRenderer : AnimationRendererBase
{
    public override string TypeKey => "random_single";
    private readonly double _brightness;

    public RandomSingleRenderer(double brightness)
    {
        _brightness = brightness;
    }

    public static RandomSingleRenderer Create(JsonElement json) =>
        new(json.GetDouble("brightness"));

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        ClearRenderShow(controller, c =>
        {
            var pixel = Random.Next(PixelCount);
            c.SetPixel(pixel, color.R, color.G, color.B, _brightness);
        });
    }
}
