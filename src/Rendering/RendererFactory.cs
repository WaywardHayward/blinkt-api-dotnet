using System.Text.Json;
using BlinktApi.Models;

namespace BlinktApi.Rendering;

public static class RendererFactory
{
    public static IAnimationRenderer? CreateRenderer(Animation animation)
    {
        if (animation.Parameters == null)
            return null;

        return animation.Type switch
        {
            "global_oscillator" => CreateGlobalOscillator(animation.Parameters.Value),
            "pixel_oscillator" => CreatePixelOscillator(animation.Parameters.Value),
            "traveling_wave" => CreateTravelingWave(animation.Parameters.Value),
            "scanner" => CreateScanner(animation.Parameters.Value),
            "fill" => CreateFill(animation.Parameters.Value),
            "sparkle" => CreateSparkle(animation.Parameters.Value),
            "rainbow_cycle" => CreateRainbowCycle(animation.Parameters.Value),
            "center_pulse" => CreateCenterPulse(animation.Parameters.Value),
            "burst_outward" => CreateBurstOutward(animation.Parameters.Value),
            "random_single" => CreateRandomSingle(animation.Parameters.Value),
            "color_cycle" => CreateColorCycle(animation.Parameters.Value),
            "bouncing_dot" => CreateBouncingDot(animation.Parameters.Value),
            "spawn_fade" => CreateSpawnFade(animation.Parameters.Value),
            "fire_flicker" => CreateFireFlicker(animation.Parameters.Value),
            "aviation_beacon" => CreateAviationBeacon(animation.Parameters.Value),
            "organic_wave" => CreateOrganicWave(animation.Parameters.Value),
            "organic_fire" => CreateOrganicFire(animation.Parameters.Value),
            "aurora" => CreateAurora(animation.Parameters.Value),
            "typing_indicator" => CreateTypingIndicator(animation.Parameters.Value),
            "matrix_rain" => CreateMatrixRain(animation.Parameters.Value),
            "random_colors" => CreateRandomColors(animation.Parameters.Value),
            "comet_trail" => CreateCometTrail(animation.Parameters.Value),
            "progress_bar" => CreateProgressBar(animation.Parameters.Value),
            "aircraft_lighting" => CreateAircraftLighting(animation.Parameters.Value),
            _ => null
        };
    }

    private static GlobalOscillatorRenderer CreateGlobalOscillator(JsonElement json)
    {
        var periodSeconds = json.GetProperty("period_seconds").GetDouble();
        var brightnessRange = json.GetProperty("brightness_range");
        var minBrightness = brightnessRange[0].GetDouble();
        var maxBrightness = brightnessRange[1].GetDouble();
        var oscillator = json.GetProperty("oscillator").GetString() ?? "sine";

        return new GlobalOscillatorRenderer(periodSeconds, minBrightness, maxBrightness, oscillator);
    }

    private static PixelOscillatorRenderer CreatePixelOscillator(JsonElement json)
    {
        var brightnessRange = json.GetProperty("brightness_range");
        var minBrightness = brightnessRange[0].GetDouble();
        var maxBrightness = brightnessRange[1].GetDouble();
        
        var speedRange = json.GetProperty("speed_range");
        var minSpeed = speedRange[0].GetDouble();
        var maxSpeed = speedRange[1].GetDouble();

        return new PixelOscillatorRenderer(minBrightness, maxBrightness, minSpeed, maxSpeed);
    }

    private static TravelingWaveRenderer CreateTravelingWave(JsonElement json)
    {
        var speed = json.GetProperty("speed").GetDouble();
        var width = json.GetProperty("width").GetInt32();
        var maxBrightness = json.GetProperty("max_brightness").GetDouble();

        return new TravelingWaveRenderer(speed, width, maxBrightness);
    }

    private static ScannerRenderer CreateScanner(JsonElement json)
    {
        var speed = json.GetProperty("speed").GetDouble();
        var maxBrightness = json.GetProperty("max_brightness").GetDouble();

        return new ScannerRenderer(speed, maxBrightness);
    }

    private static FillRenderer CreateFill(JsonElement json)
    {
        var speed = json.GetProperty("speed").GetDouble();
        var brightness = json.GetProperty("brightness").GetDouble();

        return new FillRenderer(speed, brightness);
    }

    private static SparkleRenderer CreateSparkle(JsonElement json)
    {
        var brightness = json.GetProperty("brightness").GetDouble();
        var sparsity = json.GetProperty("sparsity").GetDouble();

        return new SparkleRenderer(brightness, sparsity);
    }

    private static RainbowCycleRenderer CreateRainbowCycle(JsonElement json)
    {
        var speed = json.GetProperty("speed").GetDouble();
        var brightness = json.GetProperty("brightness").GetDouble();

        return new RainbowCycleRenderer(speed, brightness);
    }

    private static CenterPulseRenderer CreateCenterPulse(JsonElement json)
    {
        var pulseSpeed = json.GetProperty("pulse_speed").GetDouble();
        var maxBrightness = json.GetProperty("max_brightness").GetDouble();
        var beatInterval = json.GetProperty("beat_interval").GetDouble();

        return new CenterPulseRenderer(pulseSpeed, maxBrightness, beatInterval);
    }

    private static BurstOutwardRenderer CreateBurstOutward(JsonElement json)
    {
        var burstSpeed = json.GetProperty("burst_speed").GetDouble();
        var fadeSpeed = json.GetProperty("fade_speed").GetDouble();
        var maxBrightness = json.GetProperty("max_brightness").GetDouble();

        return new BurstOutwardRenderer(burstSpeed, fadeSpeed, maxBrightness);
    }

    private static RandomSingleRenderer CreateRandomSingle(JsonElement json)
    {
        var brightness = json.GetProperty("brightness").GetDouble();

        return new RandomSingleRenderer(brightness);
    }

    private static ColorCycleRenderer CreateColorCycle(JsonElement json)
    {
        var rotationSpeed = json.GetProperty("rotation_speed").GetDouble();
        var brightness = json.GetProperty("brightness").GetDouble();
        var spacing = json.GetProperty("spacing").GetDouble();

        return new ColorCycleRenderer(rotationSpeed, brightness, spacing);
    }

    private static BouncingDotRenderer CreateBouncingDot(JsonElement json)
    {
        var trailLength = json.GetProperty("trail_length").GetInt32();
        var brightnessArray = json.GetProperty("brightness").EnumerateArray()
            .Select(e => e.GetDouble())
            .ToArray();

        return new BouncingDotRenderer(trailLength, brightnessArray);
    }

    private static SpawnFadeRenderer CreateSpawnFade(JsonElement json)
    {
        var spawnChance = json.GetProperty("spawn_chance").GetDouble();
        var fadeFrames = json.GetProperty("fade_frames").GetInt32();
        var maxBrightness = json.GetProperty("max_brightness").GetDouble();

        return new SpawnFadeRenderer(spawnChance, fadeFrames, maxBrightness);
    }

    private static FireFlickerRenderer CreateFireFlicker(JsonElement json)
    {
        var baseBrightness = json.GetProperty("base_brightness").GetDouble();
        var flickerAmount = json.GetProperty("flicker_amount").GetDouble();
        var colorVariation = json.GetProperty("color_variation").GetDouble();

        return new FireFlickerRenderer(baseBrightness, flickerAmount, colorVariation);
    }

    private static AviationBeaconRenderer CreateAviationBeacon(JsonElement json)
    {
        var pattern = json.GetProperty("pattern").GetString() ?? "code1";
        var brightness = json.GetProperty("brightness").GetDouble();
        var cycleGap = json.GetProperty("cycle_gap").GetDouble();

        return new AviationBeaconRenderer(pattern, brightness, cycleGap);
    }

    private static OrganicWaveRenderer CreateOrganicWave(JsonElement json)
    {
        var baseSpeed = json.GetProperty("base_speed").GetDouble();
        var speedVariation = json.GetProperty("speed_variation").GetDouble();
        var width = json.GetProperty("width").GetInt32();
        var maxBrightness = json.GetProperty("max_brightness").GetDouble();

        return new OrganicWaveRenderer(baseSpeed, speedVariation, width, maxBrightness);
    }

    private static OrganicFireRenderer CreateOrganicFire(JsonElement json)
    {
        var baseBrightness = json.GetProperty("base_brightness").GetDouble();
        var flickerAmount = json.GetProperty("flicker_amount").GetDouble();
        var emberChance = json.GetProperty("ember_chance").GetDouble();
        var emberBrightness = json.GetProperty("ember_brightness").GetDouble();

        return new OrganicFireRenderer(baseBrightness, flickerAmount, emberChance, emberBrightness);
    }

    private static AuroraRenderer CreateAurora(JsonElement json)
    {
        var speed = json.GetProperty("speed").GetDouble();
        var brightnessVariation = json.GetProperty("brightness_variation").GetDouble();
        var maxBrightness = json.GetProperty("max_brightness").GetDouble();

        return new AuroraRenderer(speed, brightnessVariation, maxBrightness);
    }

    private static TypingIndicatorRenderer CreateTypingIndicator(JsonElement json)
    {
        var bounceSpeed = json.GetProperty("bounce_speed").GetDouble();
        var dotSpacing = json.GetProperty("dot_spacing").GetInt32();
        var brightness = json.GetProperty("brightness").GetDouble();

        return new TypingIndicatorRenderer(bounceSpeed, dotSpacing, brightness);
    }

    private static MatrixRainRenderer CreateMatrixRain(JsonElement json)
    {
        var spawnChance = json.GetProperty("spawn_chance").GetDouble();
        var fallSpeed = json.GetProperty("fall_speed").GetInt32();
        var trailLength = json.GetProperty("trail_length").GetInt32();
        var maxBrightness = json.GetProperty("max_brightness").GetDouble();

        return new MatrixRainRenderer(spawnChance, fallSpeed, trailLength, maxBrightness);
    }

    private static RandomColorsRenderer CreateRandomColors(JsonElement json)
    {
        var changeRate = json.GetProperty("change_rate").GetDouble();
        var brightness = json.GetProperty("brightness").GetDouble();

        return new RandomColorsRenderer(changeRate, brightness);
    }

    private static CometTrailRenderer CreateCometTrail(JsonElement json)
    {
        var speed = json.GetProperty("speed").GetDouble();
        var trailLength = json.GetProperty("trail_length").GetInt32();
        var headBrightness = json.GetProperty("head_brightness").GetDouble();
        var fadeRate = json.GetProperty("fade_rate").GetDouble();

        return new CometTrailRenderer(speed, trailLength, headBrightness, fadeRate);
    }

    private static ProgressBarRenderer CreateProgressBar(JsonElement json)
    {
        var brightness = json.GetProperty("brightness").GetDouble();
        var smooth = json.GetProperty("smooth").GetBoolean();

        return new ProgressBarRenderer(brightness, smooth);
    }

    private static AircraftLightingRenderer CreateAircraftLighting(JsonElement json)
    {
        var navBrightness = json.GetProperty("nav_brightness").GetDouble();
        var strobeBrightness = json.GetProperty("strobe_brightness").GetDouble();
        var strobeInterval = json.GetProperty("strobe_interval").GetDouble();
        var beaconBrightness = json.GetProperty("beacon_brightness").GetDouble();
        var beaconInterval = json.GetProperty("beacon_interval").GetDouble();

        return new AircraftLightingRenderer(navBrightness, strobeBrightness, strobeInterval, beaconBrightness, beaconInterval);
    }
}
