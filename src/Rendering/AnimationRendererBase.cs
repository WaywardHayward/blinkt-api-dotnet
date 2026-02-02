using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Base class for animation renderers with common utilities
/// </summary>
public abstract class AnimationRendererBase : IAnimationRenderer
{
    protected const int PixelCount = RenderingHelpers.PixelCount;
    protected static Random Random => RenderingHelpers.Random;
    
    public abstract void Render(BlinktController controller, Color color, double elapsedSeconds);

    /// <summary>
    /// Helper to convert HSV to RGB
    /// </summary>
    protected static Color HsvToRgb(double h, double s, double v) 
        => RenderingHelpers.HsvToRgb(h, s, v);

    /// <summary>
    /// Helper to clear, render, and show in one go
    /// </summary>
    protected void ClearRenderShow(BlinktController controller, Action<BlinktController> renderAction)
    {
        controller.Clear();
        renderAction(controller);
        controller.Show();
    }
}
