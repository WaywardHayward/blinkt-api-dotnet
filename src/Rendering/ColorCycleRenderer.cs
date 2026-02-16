using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Rainbow color cycle across pixels
/// </summary>
public class ColorCycleRenderer : AnimationRendererBase
{
    public override string TypeKey => "color_cycle";
    private readonly double _rotationSpeed;
    private readonly double _brightness;
    private readonly double _spacing;

    public ColorCycleRenderer(double rotationSpeed, double brightness, double spacing)
    {
        _rotationSpeed = rotationSpeed;
        _brightness = brightness;
        _spacing = spacing;
    }

    public static ColorCycleRenderer Create(JsonElement json)
 =>
        new(json.GetDouble("rotation_speed"), json.GetDouble("brightness"), json.GetDouble("spacing"));

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        var rotation = elapsedSeconds * _rotationSpeed * 360;
        
        for (int i = 0; i < PixelCount; i++)
        {
            var hue = (rotation + i * _spacing) % 360;
            if (hue < 0) hue += 360;
            var rgb = HsvToRgb(hue, 1.0, 1.0);
            
            controller.SetPixel(i, rgb.R, rgb.G, rgb.B, _brightness);
        }

        controller.Show();
    }
}
