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
}
