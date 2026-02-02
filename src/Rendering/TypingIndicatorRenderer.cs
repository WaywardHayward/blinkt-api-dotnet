using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Three dots bouncing like a typing indicator
/// </summary>
public class TypingIndicatorRenderer : AnimationRendererBase
{
    public override string TypeKey => "typing_indicator";
    private readonly double _bounceSpeed;
    private readonly int _dotSpacing;
    private readonly double _brightness;

    public TypingIndicatorRenderer(double bounceSpeed, int dotSpacing, double brightness)
    {
        _bounceSpeed = bounceSpeed;
        _dotSpacing = dotSpacing;
        _brightness = brightness;
    }

    public static TypingIndicatorRenderer Create(JsonElement json)
 =>
        new(json.GetDouble("bounce_speed"), json.GetInt("dot_spacing"), json.GetDouble("brightness"));

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        controller.Clear();

        // Three dots bounce up and down sequentially
        for (int dot = 0; dot < 3; dot++)
        {
            var phase = (elapsedSeconds * _bounceSpeed + dot * 0.2) * Math.PI * 2;
            var bounce = (Math.Sin(phase) + 1) / 2; // 0-1
            
            // Map bounce to brightness (0.5-1.0 range for subtle effect)
            var dotBrightness = _brightness * (0.5 + bounce * 0.5);
            
            var pixelIndex = 2 + (dot * _dotSpacing); // Start at pixel 2, space dots out
            if (pixelIndex < PixelCount)
            {
                controller.SetPixel(pixelIndex, color.R, color.G, color.B, dotBrightness);
            }
        }

        controller.Show();
    }
}
