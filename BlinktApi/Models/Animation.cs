namespace BlinktApi.Models;

public record Animation
{
    public required string Type { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? Author { get; init; }
    public string? Version { get; init; }
    public Dictionary<string, object>? Parameters { get; init; }
}

public record AnimationRequest
{
    public required string Name { get; init; }
    public string Color { get; init; } = "blue";
    public int Duration { get; init; } = 0; // 0 = infinite
}
