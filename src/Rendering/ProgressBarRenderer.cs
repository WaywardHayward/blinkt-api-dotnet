using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Progress bar that fills based on a parameter (0-100)
/// Note: This is a basic implementation - progress value needs to be passed somehow
/// </summary>
public class ProgressBarRenderer : AnimationRendererBase
{
    public override string TypeKey => "progress_bar";
    private readonly double _brightness;
    private readonly bool _smooth;

    public ProgressBarRenderer(double brightness, bool smooth)
    {
        _brightness = brightness;
        _smooth = smooth;
    }

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        // Demo mode: animate 0-100% over 10 seconds
        var progress = (elapsedSeconds % 10) / 10.0 * 100;
        
        var pixelsToFill = (progress / 100.0) * PixelCount;

        controller.Clear();

        for (int i = 0; i < PixelCount; i++)
        {
            double brightness = 0;
            
            if (_smooth)
            {
                // Smooth fade on the edge pixel
                if (i < (int)pixelsToFill)
                {
                    brightness = _brightness;
                }
                else if (i == (int)pixelsToFill)
                {
                    var fraction = pixelsToFill - (int)pixelsToFill;
                    brightness = _brightness * fraction;
                }
            }
            else
            {
                // Hard cutoff
                if (i < (int)Math.Round(pixelsToFill))
                {
                    brightness = _brightness;
                }
            }

            controller.SetPixel(i, color.R, color.G, color.B, brightness);
        }

        controller.Show();
    }
}
