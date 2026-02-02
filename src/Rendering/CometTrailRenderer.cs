using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Comet effect - bright head with long fading trail
/// </summary>
public class CometTrailRenderer : IAnimationRenderer
{
    private readonly double _speed;
    private readonly int _trailLength;
    private readonly double _headBrightness;
    private readonly double _fadeRate;
    private const int PixelCount = 8;

    public CometTrailRenderer(double speed, int trailLength, double headBrightness, double fadeRate)
    {
        _speed = speed;
        _trailLength = trailLength;
        _headBrightness = headBrightness;
        _fadeRate = fadeRate;
    }

    public void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        var position = (elapsedSeconds * _speed) % PixelCount;
        var headPixel = (int)position;

        controller.Clear();

        // Draw head
        controller.SetPixel(headPixel, color.R, color.G, color.B, _headBrightness);

        // Draw trail
        for (int i = 1; i < _trailLength; i++)
        {
            var trailPixel = (headPixel - i + PixelCount) % PixelCount;
            var brightness = _headBrightness * Math.Pow(_fadeRate, i);
            
            controller.SetPixel(trailPixel, color.R, color.G, color.B, brightness);
        }

        controller.Show();
    }
}
