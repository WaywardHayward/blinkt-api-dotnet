using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Aviation beacon patterns - rotating light with specific flash codes
/// </summary>
public class AviationBeaconRenderer : AnimationRendererBase
{
    public override string TypeKey => "aviation_beacon";
    private readonly string _pattern;
    private readonly double _brightness;
    private readonly double _cycleGap;

    public AviationBeaconRenderer(string pattern, double brightness, double cycleGap)
    {
        if (cycleGap < 0)
            throw new ArgumentOutOfRangeException(nameof(cycleGap), "Cycle gap cannot be negative.");
        
        _pattern = pattern;
        _brightness = brightness;
        _cycleGap = cycleGap;
    }

    public static AviationBeaconRenderer Create(JsonElement json)
    {
        return new(json.GetString("pattern", "code1"), json.GetDouble("brightness"), json.GetDouble("cycle_gap"));
    }

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        controller.Clear();

        var cycleTime = elapsedSeconds % (_cycleGap + GetPatternDuration());
        
        if (IsInCycleGap(cycleTime))
        {
            controller.Show();
            return;
        }

        var patternTime = cycleTime - _cycleGap;
        RenderPattern(controller, color, patternTime);
        controller.Show();
    }

    private bool IsInCycleGap(double cycleTime) => cycleTime < _cycleGap;

    private void RenderPattern(BlinktController controller, Color color, double time)
    {
        switch (_pattern)
        {
            case "code1":
                RenderCode1(controller, color, time);
                break;
            case "code2":
                RenderCode2(controller, color, time);
                break;
            case "code3":
                RenderCode3(controller, color, time);
                break;
            case "code4":
                RenderCode4(controller, color, time);
                break;
            case "fling":
                RenderFling(controller, color, time);
                break;
        }
    }

    private void RenderCode1(BlinktController controller, Color color, double time)
    {
        if (!IsFlashActive(time, 0, 0.1))
            return;

        SetAllPixels(controller, color, _brightness);
    }

    private void RenderCode2(BlinktController controller, Color color, double time)
    {
        if (!IsFlashActive(time, 0, 0.1) && !IsFlashActive(time, 0.2, 0.3))
            return;

        SetAllPixels(controller, color, _brightness);
    }

    private void RenderCode3(BlinktController controller, Color color, double time)
    {
        if (!IsFlashActive(time, 0, 0.1) && 
            !IsFlashActive(time, 0.15, 0.25) && 
            !IsFlashActive(time, 0.3, 0.4))
            return;

        SetAllPixels(controller, color, _brightness);
    }

    private void RenderCode4(BlinktController controller, Color color, double time)
    {
        if (!IsFlashActive(time, 0, 0.08) && 
            !IsFlashActive(time, 0.12, 0.2) && 
            !IsFlashActive(time, 0.24, 0.32) && 
            !IsFlashActive(time, 0.36, 0.44))
            return;

        SetAllPixels(controller, color, _brightness);
    }

    private void RenderFling(BlinktController controller, Color color, double time)
    {
        var position = (time * 3) % PixelCount; // 3 rotations per second
        var pixel = (int)position;
        
        // Use RGB scaling for smooth brightness
        SetPixelSmooth(controller, pixel, color, _brightness);
        
        var prevPixel = (pixel - 1 + PixelCount) % PixelCount;
        SetPixelSmooth(controller, prevPixel, color, _brightness * 0.3);
    }

    private bool IsFlashActive(double time, double start, double end) => 
        time >= start && time < end;

    private double GetPatternDuration() => _pattern switch
    {
        "code1" => 0.1,
        "code2" => 0.3,
        "code3" => 0.4,
        "code4" => 0.44,
        "fling" => 1.0,
        _ => 0.5
    };
}
