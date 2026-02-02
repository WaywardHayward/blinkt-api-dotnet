using System.Text.Json;

namespace BlinktApi.Rendering;

/// <summary>
/// Base factory implementation that delegates to the renderer's static Create method
/// </summary>
public abstract class RendererFactoryBase<TRenderer> : IRendererFactory
    where TRenderer : AnimationRendererBase
{
    public abstract string TypeKey { get; }
    
    public IAnimationRenderer Create(JsonElement json) => CreateTyped(json);
    
    protected abstract TRenderer CreateTyped(JsonElement json);
}

// Individual factory implementations - one per renderer

public class RainbowCycleFactory : RendererFactoryBase<RainbowCycleRenderer>
{
    public override string TypeKey => "rainbow_cycle";
    protected override RainbowCycleRenderer CreateTyped(JsonElement json) => RainbowCycleRenderer.Create(json);
}

public class AircraftLightingFactory : RendererFactoryBase<AircraftLightingRenderer>
{
    public override string TypeKey => "aircraft_lighting";
    protected override AircraftLightingRenderer CreateTyped(JsonElement json) => AircraftLightingRenderer.Create(json);
}

public class ScannerFactory : RendererFactoryBase<ScannerRenderer>
{
    public override string TypeKey => "scanner";
    protected override ScannerRenderer CreateTyped(JsonElement json) => ScannerRenderer.Create(json);
}

public class FillFactory : RendererFactoryBase<FillRenderer>
{
    public override string TypeKey => "fill";
    protected override FillRenderer CreateTyped(JsonElement json) => FillRenderer.Create(json);
}

public class SparkleFactory : RendererFactoryBase<SparkleRenderer>
{
    public override string TypeKey => "sparkle";
    protected override SparkleRenderer CreateTyped(JsonElement json) => SparkleRenderer.Create(json);
}

public class RandomSingleFactory : RendererFactoryBase<RandomSingleRenderer>
{
    public override string TypeKey => "random_single";
    protected override RandomSingleRenderer CreateTyped(JsonElement json) => RandomSingleRenderer.Create(json);
}

public class RandomColorsFactory : RendererFactoryBase<RandomColorsRenderer>
{
    public override string TypeKey => "random_colors";
    protected override RandomColorsRenderer CreateTyped(JsonElement json) => RandomColorsRenderer.Create(json);
}

public class CometTrailFactory : RendererFactoryBase<CometTrailRenderer>
{
    public override string TypeKey => "comet_trail";
    protected override CometTrailRenderer CreateTyped(JsonElement json) => CometTrailRenderer.Create(json);
}

public class ProgressBarFactory : RendererFactoryBase<ProgressBarRenderer>
{
    public override string TypeKey => "progress_bar";
    protected override ProgressBarRenderer CreateTyped(JsonElement json) => ProgressBarRenderer.Create(json);
}

public class GlobalOscillatorFactory : RendererFactoryBase<GlobalOscillatorRenderer>
{
    public override string TypeKey => "global_oscillator";
    protected override GlobalOscillatorRenderer CreateTyped(JsonElement json) => GlobalOscillatorRenderer.Create(json);
}

public class PixelOscillatorFactory : RendererFactoryBase<PixelOscillatorRenderer>
{
    public override string TypeKey => "pixel_oscillator";
    protected override PixelOscillatorRenderer CreateTyped(JsonElement json) => PixelOscillatorRenderer.Create(json);
}

public class TravelingWaveFactory : RendererFactoryBase<TravelingWaveRenderer>
{
    public override string TypeKey => "traveling_wave";
    protected override TravelingWaveRenderer CreateTyped(JsonElement json) => TravelingWaveRenderer.Create(json);
}

public class CenterPulseFactory : RendererFactoryBase<CenterPulseRenderer>
{
    public override string TypeKey => "center_pulse";
    protected override CenterPulseRenderer CreateTyped(JsonElement json) => CenterPulseRenderer.Create(json);
}

public class BurstOutwardFactory : RendererFactoryBase<BurstOutwardRenderer>
{
    public override string TypeKey => "burst_outward";
    protected override BurstOutwardRenderer CreateTyped(JsonElement json) => BurstOutwardRenderer.Create(json);
}

public class ColorCycleFactory : RendererFactoryBase<ColorCycleRenderer>
{
    public override string TypeKey => "color_cycle";
    protected override ColorCycleRenderer CreateTyped(JsonElement json) => ColorCycleRenderer.Create(json);
}

public class BouncingDotFactory : RendererFactoryBase<BouncingDotRenderer>
{
    public override string TypeKey => "bouncing_dot";
    protected override BouncingDotRenderer CreateTyped(JsonElement json) => BouncingDotRenderer.Create(json);
}

public class SpawnFadeFactory : RendererFactoryBase<SpawnFadeRenderer>
{
    public override string TypeKey => "spawn_fade";
    protected override SpawnFadeRenderer CreateTyped(JsonElement json) => SpawnFadeRenderer.Create(json);
}

public class FireFlickerFactory : RendererFactoryBase<FireFlickerRenderer>
{
    public override string TypeKey => "fire_flicker";
    protected override FireFlickerRenderer CreateTyped(JsonElement json) => FireFlickerRenderer.Create(json);
}

public class AviationBeaconFactory : RendererFactoryBase<AviationBeaconRenderer>
{
    public override string TypeKey => "aviation_beacon";
    protected override AviationBeaconRenderer CreateTyped(JsonElement json) => AviationBeaconRenderer.Create(json);
}

public class OrganicWaveFactory : RendererFactoryBase<OrganicWaveRenderer>
{
    public override string TypeKey => "organic_wave";
    protected override OrganicWaveRenderer CreateTyped(JsonElement json) => OrganicWaveRenderer.Create(json);
}

public class OrganicFireFactory : RendererFactoryBase<OrganicFireRenderer>
{
    public override string TypeKey => "organic_fire";
    protected override OrganicFireRenderer CreateTyped(JsonElement json) => OrganicFireRenderer.Create(json);
}

public class AuroraFactory : RendererFactoryBase<AuroraRenderer>
{
    public override string TypeKey => "aurora";
    protected override AuroraRenderer CreateTyped(JsonElement json) => AuroraRenderer.Create(json);
}

public class TypingIndicatorFactory : RendererFactoryBase<TypingIndicatorRenderer>
{
    public override string TypeKey => "typing_indicator";
    protected override TypingIndicatorRenderer CreateTyped(JsonElement json) => TypingIndicatorRenderer.Create(json);
}

public class MatrixRainFactory : RendererFactoryBase<MatrixRainRenderer>
{
    public override string TypeKey => "matrix_rain";
    protected override MatrixRainRenderer CreateTyped(JsonElement json) => MatrixRainRenderer.Create(json);
}
