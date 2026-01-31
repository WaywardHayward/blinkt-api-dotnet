using System.Drawing;
using System.Text.Json;

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
    public string Color { get; init; } = "blue";
    public int Duration { get; init; } = 0; // 0 = infinite
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

    public static Color Parse(string colorName)
    {
        return NamedColors.TryGetValue(colorName.ToLower(), out var color)
            ? color
            : Color.FromArgb(0, 0, 255); // Default to blue
    }
}
