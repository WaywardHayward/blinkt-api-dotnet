using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Aviation beacon patterns - rotating light with specific flash codes
/// </summary>
public class AviationBeaconRenderer : AnimationRendererBase
{
    private readonly string _pattern;
    private readonly double _brightness;
    private readonly double _cycleGap;

    public AviationBeaconRenderer(string pattern, double brightness, double cycleGap)
    {
        _pattern = pattern;
        _brightness = brightness;
        _cycleGap = cycleGap;
    }

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        controller.Clear();

        var cycleTime = elapsedSeconds % (_cycleGap + GetPatternDuration());
        
        if (cycleTime < _cycleGap)
        {
            // Gap between cycles
            controller.Show();
            return;
        }

        var patternTime = cycleTime - _cycleGap;

        switch (_pattern)
        {
            case "code1":
                RenderCode1(controller, color, patternTime);
                break;
            case "code2":
                RenderCode2(controller, color, patternTime);
                break;
            case "code3":
                RenderCode3(controller, color, patternTime);
                break;
            case "code4":
                RenderCode4(controller, color, patternTime);
                break;
            case "fling":
                RenderFling(controller, color, patternTime);
                break;
        }

        controller.Show();
    }

    private void RenderCode1(BlinktController controller, Color color, double time)
    {
        // Single flash
        if (time < 0.1)
        {
            for (int i = 0; i < PixelCount; i++)
                controller.SetPixel(i, color.R, color.G, color.B, _brightness);
        }
    }

    private void RenderCode2(BlinktController controller, Color color, double time)
    {
        // Double flash: flash-gap-flash
        if (time < 0.1 || (time > 0.2 && time < 0.3))
        {
            for (int i = 0; i < PixelCount; i++)
                controller.SetPixel(i, color.R, color.G, color.B, _brightness);
        }
    }

    private void RenderCode3(BlinktController controller, Color color, double time)
    {
        // Triple flash
        if (time < 0.1 || (time > 0.15 && time < 0.25) || (time > 0.3 && time < 0.4))
        {
            for (int i = 0; i < PixelCount; i++)
                controller.SetPixel(i, color.R, color.G, color.B, _brightness);
        }
    }

    private void RenderCode4(BlinktController controller, Color color, double time)
    {
        // Quadruple flash
        if (time < 0.08 || (time > 0.12 && time < 0.2) || 
            (time > 0.24 && time < 0.32) || (time > 0.36 && time < 0.44))
        {
            for (int i = 0; i < PixelCount; i++)
                controller.SetPixel(i, color.R, color.G, color.B, _brightness);
        }
    }

    private void RenderFling(BlinktController controller, Color color, double time)
    {
        // Rotating beacon that sweeps around
        var position = (time * 3) % PixelCount; // 3 rotations per second
        var pixel = (int)position;
        
        controller.SetPixel(pixel, color.R, color.G, color.B, _brightness);
        
        // Add trailing fade
        var prevPixel = (pixel - 1 + PixelCount) % PixelCount;
        controller.SetPixel(prevPixel, color.R, color.G, color.B, _brightness * 0.3);
    }

    private double GetPatternDuration()
    {
        return _pattern switch
        {
            "code1" => 0.1,
            "code2" => 0.3,
            "code3" => 0.4,
            "code4" => 0.44,
            "fling" => 1.0,
            _ => 0.5
        };
    }
}
