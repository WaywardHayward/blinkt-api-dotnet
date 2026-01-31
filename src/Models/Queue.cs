using System.Drawing;

namespace BlinktApi.Models;

public record QueuedAnimation
{
    public required string Name { get; init; }
    public required Color Color { get; init; }
    public required int DurationSeconds { get; init; }
}

public record QueueRequest
{
    public required List<QueueItem> Queue { get; init; }
}

public record QueueItem
{
    public required string Name { get; init; }
    public string? Color { get; init; }
    public RgbColor? Rgb { get; init; }
    public int Duration { get; init; } = 3; // Default 3 seconds
}
