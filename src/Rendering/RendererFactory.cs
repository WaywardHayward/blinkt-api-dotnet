using System.Text.Json;
using BlinktApi.Models;

namespace BlinktApi.Rendering;

public class RendererFactory
{
    private readonly Dictionary<string, IRendererFactory> _factories;

    public RendererFactory(IEnumerable<IRendererFactory> factories)
    {
        _factories = factories.ToDictionary(f => f.TypeKey, f => f);
    }

    public IAnimationRenderer? CreateRenderer(Animation animation)
    {
        if (animation.Parameters == null)
            return null;

        return _factories.TryGetValue(animation.Type, out var factory)
            ? factory.Create(animation.Parameters.Value)
            : null;
    }
}
