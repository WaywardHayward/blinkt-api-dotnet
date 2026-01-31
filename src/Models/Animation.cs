using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlinktApi.Models;

public record Animation
{
    public required string Name { get; init; }
    public required string Type { get; init; }
    public string? Description { get; init; }
    public string? Author { get; init; }
    public string? Version { get; init; }
    public JsonElement? Parameters { get; init; }
}

public record AnimationRequest
{
    public required string Name { get; init; }
    
    [JsonPropertyName("color")]
    public string? ColorString { get; init; }
    
    [JsonPropertyName("rgb")]
    public RgbColor? Rgb { get; init; }
    
    public int Duration { get; init; } = 0; // 0 = infinite
}

public record RgbColor
{
    public byte R { get; init; }
    public byte G { get; init; }
    public byte B { get; init; }
}

public record AnimationState
{
    public required string Name { get; init; }
    public required Color Color { get; init; }
    public required DateTime StartTime { get; init; }
    public int DurationSeconds { get; init; } // 0 = infinite
    
    public bool IsExpired => DurationSeconds > 0 && 
        (DateTime.UtcNow - StartTime).TotalSeconds >= DurationSeconds;
}

public static class ColorHelper
{
    private static readonly Dictionary<string, Color> NamedColors = new()
    {
        ["red"] = Color.FromArgb(255, 0, 0),
        ["green"] = Color.FromArgb(0, 255, 0),
        ["blue"] = Color.FromArgb(0, 0, 255),
        ["cyan"] = Color.FromArgb(0, 255, 255),
        ["yellow"] = Color.FromArgb(255, 255, 0),
        ["magenta"] = Color.FromArgb(255, 0, 255),
        ["orange"] = Color.FromArgb(255, 165, 0),
        ["purple"] = Color.FromArgb(128, 0, 128),
        ["pink"] = Color.FromArgb(255, 192, 203),
        ["white"] = Color.FromArgb(255, 255, 255),
    };

    public static Color Parse(string colorInput)
    {
        var input = colorInput.Trim();
        
        // Try hex format: #RRGGBB or RRGGBB
        if (input.StartsWith('#'))
            input = input[1..];
            
        if (input.Length == 6 && int.TryParse(input, System.Globalization.NumberStyles.HexNumber, null, out var hexValue))
        {
            var r = (byte)((hexValue >> 16) & 0xFF);
            var g = (byte)((hexValue >> 8) & 0xFF);
            var b = (byte)(hexValue & 0xFF);
            return Color.FromArgb(r, g, b);
        }
        
        // Try named color
        if (NamedColors.TryGetValue(input.ToLower(), out var color))
            return color;
        
        throw new ArgumentException($"Invalid color: '{colorInput}'. Use a named color (red, blue, etc.) or hex format (#FF0000)");
    }
    
    public static Color FromRgb(byte r, byte g, byte b)
    {
        return Color.FromArgb(r, g, b);
    }
}
