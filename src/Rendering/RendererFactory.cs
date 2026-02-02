using System.Reflection;
using System.Text.Json;
using BlinktApi.Models;

namespace BlinktApi.Rendering;

public static class RendererFactory
{
    private static readonly Dictionary<string, Func<JsonElement, IAnimationRenderer>> _factories = new();
    private static bool _initialized = false;

    public static IAnimationRenderer? CreateRenderer(Animation animation)
    {
        if (!_initialized)
            Initialize();

        if (animation.Parameters == null)
            return null;

        // Use reflection-based factory
        return _factories.TryGetValue(animation.Type, out var factory)
            ? factory(animation.Parameters.Value)
            : null;
    }

    private static void Initialize()
    {
        // Discover all renderer types with a static Create method
        var rendererTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(AnimationRendererBase).IsAssignableFrom(t));

        foreach (var type in rendererTypes)
        {
            // Look for static Create method
            var createMethod = type.GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
            if (createMethod == null || createMethod.ReturnType != type)
                continue;

            // Get TypeKey property
            var typeKeyProperty = type.GetProperty("TypeKey", BindingFlags.Public | BindingFlags.Instance);
            if (typeKeyProperty == null)
                continue;

            // Create a temporary instance to get the TypeKey value
            // Use FormatterServices to bypass constructor (unsafe but works for getting static data)
            var instance = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
            var typeKey = typeKeyProperty.GetValue(instance) as string;
            
            if (string.IsNullOrEmpty(typeKey))
                continue;

            // Create factory function
            _factories[typeKey] = (JsonElement json) =>
            {
                return (IAnimationRenderer)createMethod.Invoke(null, new object[] { json })!;
            };
        }

        _initialized = true;
    }
}
