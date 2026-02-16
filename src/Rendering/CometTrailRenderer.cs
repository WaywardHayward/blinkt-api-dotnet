using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Comet effect - bright head with long fading trail
/// </summary>
public class CometTrailRenderer : AnimationRendererBase
{
    public override string TypeKey => "comet_trail";
    private readonly double _speed;
    private readonly int _trailLength;
    private readonly double _headBrightness;
    private readonly double _fadeRate;

    public CometTrailRenderer(double speed, int trailLength, double headBrightness, double fadeRate)
    {
        if (headBrightness < 0 || headBrightness > 1)
            throw new ArgumentOutOfRangeException(nameof(headBrightness), "Head brightness must be between 0.0 and 1.0.");
        if (fadeRate < 0 || fadeRate > 1)
            throw new ArgumentOutOfRangeException(nameof(fadeRate), "Fade rate must be between 0.0 and 1.0.");
        
        _speed = speed;
        _trailLength = trailLength;
        _headBrightness = headBrightness;
        _fadeRate = fadeRate;
    }

    public static CometTrailRenderer Create(JsonElement json) =>
        new(json.GetDouble("speed"), json.GetInt("trail_length"), json.GetDouble("head_brightness"), json.GetDouble("fade_rate"));

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        var position = (elapsedSeconds * _speed) % PixelCount;
        if (position < 0) position += PixelCount;
        var headPixel = (int)position;

        controller.Clear();

        // Draw head - use RGB scaling for smooth brightness
        SetPixelSmooth(controller, headPixel, color, _headBrightness);

        // Draw trail - use RGB scaling for smooth fading
        for (int i = 1; i < _trailLength; i++)
        {
            var trailPixel = (headPixel - i + PixelCount) % PixelCount;
            var brightness = _headBrightness * Math.Pow(_fadeRate, i);
            
            SetPixelSmooth(controller, trailPixel, color, brightness);
        }

        controller.Show();
    }
}
