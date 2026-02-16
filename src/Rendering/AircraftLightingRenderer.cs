using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Complete aircraft lighting system - navigation lights, strobes, and tail beacon
/// </summary>
public class AircraftLightingRenderer : AnimationRendererBase
{
    public override string TypeKey => "aircraft_lighting";
    
    private readonly double _navBrightness;
    private readonly double _strobeBrightness;
    private readonly double _strobeInterval;
    private readonly double _beaconBrightness;
    private readonly double _beaconInterval;

    public AircraftLightingRenderer(
        double navBrightness, 
        double strobeBrightness, 
        double strobeInterval,
        double beaconBrightness,
        double beaconInterval)
    {
        if (strobeInterval <= 0)
            throw new ArgumentOutOfRangeException(nameof(strobeInterval), "Strobe interval must be positive.");
        if (beaconInterval <= 0)
            throw new ArgumentOutOfRangeException(nameof(beaconInterval), "Beacon interval must be positive.");
        
        _navBrightness = navBrightness;
        _strobeBrightness = strobeBrightness;
        _strobeInterval = strobeInterval;
        _beaconBrightness = beaconBrightness;
        _beaconInterval = beaconInterval;
    }

    public static AircraftLightingRenderer Create(JsonElement json) =>
        new(json.GetDouble("nav_brightness"), 
            json.GetDouble("strobe_brightness"), 
            json.GetDouble("strobe_interval"), 
            json.GetDouble("beacon_brightness"), 
            json.GetDouble("beacon_interval"));

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        controller.Clear();

        // Pixel layout (8 LEDs):
        // 0: Red (port/left wing)
        // 1-2: White strobes (left)
        // 3-4: White strobes (right)
        // 5: Green (starboard/right wing)
        // 6-7: White tail beacon

        RenderNavigationLights(controller);
        RenderWingStrobes(controller, elapsedSeconds);
        RenderTailBeacon(controller, elapsedSeconds);

        controller.Show();
    }

    private void RenderNavigationLights(BlinktController controller)
    {
        controller.SetPixel(0, 255, 0, 0, _navBrightness);     // Red port
        controller.SetPixel(5, 0, 255, 0, _navBrightness);     // Green starboard
    }

    private void RenderWingStrobes(BlinktController controller, double elapsedSeconds)
    {
        var strobePhase = (elapsedSeconds % _strobeInterval) / _strobeInterval;
        
        if (!IsStrobeFlashing(strobePhase))
            return;

        SetPixels(controller, 255, 255, 255, _strobeBrightness, 1, 2, 3, 4);
    }

    private bool IsStrobeFlashing(double phase) =>
        phase < 0.05 || (phase > 0.1 && phase < 0.15);

    private void RenderTailBeacon(BlinktController controller, double elapsedSeconds)
    {
        var beaconPhase = (elapsedSeconds % _beaconInterval) / _beaconInterval;
        
        if (beaconPhase >= 0.1)
            return;

        SetPixels(controller, 255, 255, 255, _beaconBrightness, 6, 7);
    }
}
