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

    /// <summary>
    /// Execute a function for each pixel
    /// </summary>
    protected void ForEachPixel(Action<int> action)
    {
        for (int i = 0; i < PixelCount; i++)
        {
            action(i);
        }
    }

    /// <summary>
    /// Execute a function for each pixel with the controller
    /// </summary>
    protected void ForEachPixel(BlinktController controller, Action<BlinktController, int> action)
    {
        for (int i = 0; i < PixelCount; i++)
        {
            action(controller, i);
        }
    }

    /// <summary>
    /// Set the same color/brightness on multiple pixels
    /// </summary>
    protected void SetPixels(BlinktController controller, Color color, double brightness, params int[] pixels)
    {
        foreach (var pixel in pixels)
        {
            controller.SetPixel(pixel, color.R, color.G, color.B, brightness);
        }
    }

    /// <summary>
    /// Set the same RGB/brightness on multiple pixels
    /// </summary>
    protected void SetPixels(BlinktController controller, byte r, byte g, byte b, double brightness, params int[] pixels)
    {
        foreach (var pixel in pixels)
        {
            controller.SetPixel(pixel, r, g, b, brightness);
        }
    }

    /// <summary>
    /// Set a range of pixels to the same color/brightness
    /// </summary>
    protected void SetPixelRange(BlinktController controller, Color color, double brightness, int start, int count)
    {
        for (int i = start; i < start + count && i < PixelCount; i++)
        {
            controller.SetPixel(i, color.R, color.G, color.B, brightness);
        }
    }

    /// <summary>
    /// Set all pixels to the same color/brightness
    /// </summary>
    protected void SetAllPixels(BlinktController controller, Color color, double brightness)
    {
        ForEachPixel(i => controller.SetPixel(i, color.R, color.G, color.B, brightness));
    }
}
