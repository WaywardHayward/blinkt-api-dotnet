using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Base class for animation renderers with common utilities
/// </summary>
public abstract class AnimationRendererBase : IAnimationRenderer
{
    protected const int PixelCount = RenderingHelpers.PixelCount;
    protected static Random Random => RenderingHelpers.Random;
    
    /// <summary>
    /// Unique key identifying this renderer type (e.g., "rainbow_cycle", "aircraft_lighting")
    /// Override this in derived classes to enable reflection-based discovery
    /// </summary>
    public virtual string? TypeKey => null;
    
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
        if (start < 0 || count < 0)
            throw new ArgumentOutOfRangeException("start/count must be non-negative.");
        
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

    /// <summary>
    /// Set a pixel with smooth brightness by scaling RGB values instead of using hardware brightness.
    /// This provides 256 levels of brightness instead of the APA102's 5-bit (32 levels),
    /// eliminating visible stepping/flickering in brightness transitions.
    /// </summary>
    /// <param name="controller">The Blinkt controller</param>
    /// <param name="index">Pixel index (0-7)</param>
    /// <param name="color">Base color</param>
    /// <param name="brightness">Brightness 0.0-1.0</param>
    /// <param name="hardwareBrightness">Hardware brightness to use (default 1.0)</param>
    protected static void SetPixelSmooth(BlinktController controller, int index, Color color, double brightness, double hardwareBrightness = 1.0)
    {
        var r = (byte)Math.Clamp(color.R * brightness, 0, 255);
        var g = (byte)Math.Clamp(color.G * brightness, 0, 255);
        var b = (byte)Math.Clamp(color.B * brightness, 0, 255);
        controller.SetPixel(index, r, g, b, hardwareBrightness);
    }

    /// <summary>
    /// Set all pixels with smooth brightness by scaling RGB values.
    /// </summary>
    protected static void SetAllSmooth(BlinktController controller, Color color, double brightness, double hardwareBrightness = 1.0)
    {
        var r = (byte)Math.Clamp(color.R * brightness, 0, 255);
        var g = (byte)Math.Clamp(color.G * brightness, 0, 255);
        var b = (byte)Math.Clamp(color.B * brightness, 0, 255);
        controller.SetAll(r, g, b, hardwareBrightness);
    }
}

/// <summary>
/// JSON parsing extension methods for renderer factories
/// </summary>
public static class JsonElementExtensions
{
    public static double GetDouble(this JsonElement json, string property) =>
        json.GetProperty(property).GetDouble();

    public static int GetInt(this JsonElement json, string property) =>
        json.GetProperty(property).GetInt32();

    public static bool GetBool(this JsonElement json, string property) =>
        json.GetProperty(property).GetBoolean();

    public static string GetString(this JsonElement json, string property, string defaultValue = "") =>
        json.TryGetProperty(property, out var prop)
            ? prop.GetString() ?? defaultValue
            : defaultValue;

    public static (double min, double max) GetRange(this JsonElement json, string property)
    {
        if (!json.TryGetProperty(property, out var range) || range.GetArrayLength() < 2)
            throw new JsonException($"Expected '{property}' to be an array with at least 2 elements.");
        return (range[0].GetDouble(), range[1].GetDouble());
    }

    public static double[] GetDoubleArray(this JsonElement json, string property) =>
        json.GetProperty(property).EnumerateArray().Select(e => e.GetDouble()).ToArray();
}
