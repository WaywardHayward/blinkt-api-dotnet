using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Bouncing dot scanner effect with trailing fade
/// </summary>
public class BouncingDotRenderer : AnimationRendererBase
{
    public override string TypeKey => "bouncing_dot";
    private readonly int _trailLength;
    private readonly double[] _brightness;

    public BouncingDotRenderer(int trailLength, double[] brightness)
    {
        _trailLength = trailLength;
        _brightness = brightness;
    }

    public static BouncingDotRenderer Create(JsonElement json)
    {
        return new(json.GetInt("trail_length"), json.GetDoubleArray("brightness"));
    }

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        // Calculate position (bounce back and forth)
        var cycleLength = (PixelCount - 1) * 2;
        var position = (elapsedSeconds * 10) % cycleLength; // Speed multiplier of 10
        
        int headPosition;
        if (position < PixelCount - 1)
        {
            // Moving forward (0 -> 7)
            headPosition = (int)position;
        }
        else
        {
            // Moving backward (7 -> 0)
            headPosition = (PixelCount - 1) - (int)(position - (PixelCount - 1));
        }

        controller.Clear();

        // Draw head and trail
        for (int i = 0; i < _trailLength && i < _brightness.Length; i++)
        {
            var pixelPos = headPosition - i;
            if (pixelPos >= 0 && pixelPos < PixelCount)
            {
                controller.SetPixel(pixelPos, color.R, color.G, color.B, _brightness[i]);
            }
        }

        controller.Show();
    }
}
