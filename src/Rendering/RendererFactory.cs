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

        var json = animation.Parameters.Value;

        // Try reflection-based factory first
        if (_factories.TryGetValue(animation.Type, out var factory))
            return factory(json);

        // Fallback to legacy inline creation
        return animation.Type switch
        {
            "global_oscillator" => new GlobalOscillatorRenderer(
                json.GetDouble("period_seconds"),
                json.GetRange("brightness_range").min,
                json.GetRange("brightness_range").max,
                json.GetString("oscillator", "sine")),
            
            "pixel_oscillator" => new PixelOscillatorRenderer(
                json.GetRange("brightness_range").min,
                json.GetRange("brightness_range").max,
                json.GetRange("speed_range").min,
                json.GetRange("speed_range").max),
            
            "traveling_wave" => new TravelingWaveRenderer(
                json.GetDouble("speed"),
                json.GetInt("width"),
                json.GetDouble("max_brightness")),
            
            "scanner" => new ScannerRenderer(
                json.GetDouble("speed"),
                json.GetDouble("max_brightness")),
            
            "fill" => new FillRenderer(
                json.GetDouble("speed"),
                json.GetDouble("brightness")),
            
            "sparkle" => new SparkleRenderer(
                json.GetDouble("brightness"),
                json.GetDouble("sparsity")),
            
            "center_pulse" => new CenterPulseRenderer(
                json.GetDouble("pulse_speed"),
                json.GetDouble("max_brightness"),
                json.GetDouble("beat_interval")),
            
            "burst_outward" => new BurstOutwardRenderer(
                json.GetDouble("burst_speed"),
                json.GetDouble("fade_speed"),
                json.GetDouble("max_brightness")),
            
            "random_single" => new RandomSingleRenderer(
                json.GetDouble("brightness")),
            
            "color_cycle" => new ColorCycleRenderer(
                json.GetDouble("rotation_speed"),
                json.GetDouble("brightness"),
                json.GetDouble("spacing")),
            
            "bouncing_dot" => new BouncingDotRenderer(
                json.GetInt("trail_length"),
                json.GetDoubleArray("brightness")),
            
            "spawn_fade" => new SpawnFadeRenderer(
                json.GetDouble("spawn_chance"),
                json.GetInt("fade_frames"),
                json.GetDouble("max_brightness")),
            
            "fire_flicker" => new FireFlickerRenderer(
                json.GetDouble("base_brightness"),
                json.GetDouble("flicker_amount"),
                json.GetDouble("color_variation")),
            
            "aviation_beacon" => new AviationBeaconRenderer(
                json.GetString("pattern", "code1"),
                json.GetDouble("brightness"),
                json.GetDouble("cycle_gap")),
            
            "organic_wave" => new OrganicWaveRenderer(
                json.GetDouble("base_speed"),
                json.GetDouble("speed_variation"),
                json.GetInt("width"),
                json.GetDouble("max_brightness")),
            
            "organic_fire" => new OrganicFireRenderer(
                json.GetDouble("base_brightness"),
                json.GetDouble("flicker_amount"),
                json.GetDouble("ember_chance"),
                json.GetDouble("ember_brightness")),
            
            "aurora" => new AuroraRenderer(
                json.GetDouble("speed"),
                json.GetDouble("brightness_variation"),
                json.GetDouble("max_brightness")),
            
            "typing_indicator" => new TypingIndicatorRenderer(
                json.GetDouble("bounce_speed"),
                json.GetInt("dot_spacing"),
                json.GetDouble("brightness")),
            
            "matrix_rain" => new MatrixRainRenderer(
                json.GetDouble("spawn_chance"),
                json.GetInt("fall_speed"),
                json.GetInt("trail_length"),
                json.GetDouble("max_brightness")),
            
            "random_colors" => new RandomColorsRenderer(
                json.GetDouble("change_rate"),
                json.GetDouble("brightness")),
            
            "comet_trail" => new CometTrailRenderer(
                json.GetDouble("speed"),
                json.GetInt("trail_length"),
                json.GetDouble("head_brightness"),
                json.GetDouble("fade_rate")),
            
            "progress_bar" => new ProgressBarRenderer(
                json.GetDouble("brightness"),
                json.GetBool("smooth")),
            
            _ => null
        };
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

            // Get the TypeKey from a temporary instance
            var instance = Activator.CreateInstance(type, GetDefaultConstructorArgs(type));
            if (instance is not AnimationRendererBase renderer || renderer.TypeKey == null)
                continue;

            // Create factory function
            _factories[renderer.TypeKey] = (JsonElement json) =>
            {
                return (IAnimationRenderer)createMethod.Invoke(null, new object[] { json })!;
            };
        }

        _initialized = true;
    }

    private static object[] GetDefaultConstructorArgs(Type type)
    {
        var constructor = type.GetConstructors().FirstOrDefault();
        if (constructor == null)
            return Array.Empty<object>();

        return constructor.GetParameters()
            .Select(p => p.ParameterType.IsValueType ? Activator.CreateInstance(p.ParameterType)! : null!)
            .ToArray();
    }
}
