using System.Drawing;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Complete aircraft lighting system - navigation lights, strobes, and tail beacon
/// </summary>
public class AircraftLightingRenderer : IAnimationRenderer
{
    private readonly double _navBrightness;
    private readonly double _strobeBrightness;
    private readonly double _strobeInterval;
    private readonly double _beaconBrightness;
    private readonly double _beaconInterval;
    private const int PixelCount = 8;

    public AircraftLightingRenderer(
        double navBrightness, 
        double strobeBrightness, 
        double strobeInterval,
        double beaconBrightness,
        double beaconInterval)
    {
        _navBrightness = navBrightness;
        _strobeBrightness = strobeBrightness;
        _strobeInterval = strobeInterval;
        _beaconBrightness = beaconBrightness;
        _beaconInterval = beaconInterval;
    }

    public void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        controller.Clear();

        // Pixel layout (8 LEDs):
        // 0: Red (port/left wing)
        // 1-2: White strobes (left)
        // 3-4: White strobes (right)
        // 5: Green (starboard/right wing)
        // 6-7: White tail beacon

        // Navigation lights - always on
        controller.SetPixel(0, 255, 0, 0, _navBrightness);     // Red port
        controller.SetPixel(5, 0, 255, 0, _navBrightness);     // Green starboard

        // Wing strobes - flash together
        var strobePhase = (elapsedSeconds % _strobeInterval) / _strobeInterval;
        if (strobePhase < 0.05) // Quick double flash
        {
            controller.SetPixel(1, 255, 255, 255, _strobeBrightness);
            controller.SetPixel(2, 255, 255, 255, _strobeBrightness);
            controller.SetPixel(3, 255, 255, 255, _strobeBrightness);
            controller.SetPixel(4, 255, 255, 255, _strobeBrightness);
        }
        else if (strobePhase > 0.1 && strobePhase < 0.15)
        {
            controller.SetPixel(1, 255, 255, 255, _strobeBrightness);
            controller.SetPixel(2, 255, 255, 255, _strobeBrightness);
            controller.SetPixel(3, 255, 255, 255, _strobeBrightness);
            controller.SetPixel(4, 255, 255, 255, _strobeBrightness);
        }

        // Tail beacon - rotating flash
        var beaconPhase = (elapsedSeconds % _beaconInterval) / _beaconInterval;
        if (beaconPhase < 0.1)
        {
            controller.SetPixel(6, 255, 255, 255, _beaconBrightness);
            controller.SetPixel(7, 255, 255, 255, _beaconBrightness);
        }

        controller.Show();
    }
}
