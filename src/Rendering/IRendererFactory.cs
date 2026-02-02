using System.Text.Json;

namespace BlinktApi.Rendering;

/// <summary>
/// Factory interface for creating animation renderers from JSON configuration
/// </summary>
public interface IRendererFactory
{
    /// <summary>
    /// The animation type key this factory handles (e.g., "rainbow_cycle", "scanner")
    /// </summary>
    string TypeKey { get; }
    
    /// <summary>
    /// Create a renderer instance from JSON parameters
    /// </summary>
    IAnimationRenderer Create(JsonElement json);
}
