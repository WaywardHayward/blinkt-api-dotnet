using System.Drawing;
using System.Text.Json;
using BlinktApi.Hardware;

namespace BlinktApi.Rendering;

/// <summary>
/// Matrix-style digital rain effect - like the iconic falling code
/// Features:
/// - Background shimmer with random green intensities
/// - Occasional bright white "leading" drops traveling across
/// - Random white "flash" pops that fade to green
/// - Multiple independent timing layers for organic feel
/// </summary>
public class MatrixRainRenderer : AnimationRendererBase
{
    public override string TypeKey => "matrix_rain";
    
    private readonly double _maxBrightness;
    private readonly double _dropChance;
    private readonly double _flashChance;
    private readonly int _trailLength;
    
    // Per-pixel state for background shimmer
    private readonly double[] _phases;
    private readonly double[] _speeds;
    private readonly double[] _targetBrightness;
    private readonly double[] _currentBrightness;
    
    // Active "drops" traveling across (bright leading pixels with trails)
    private readonly List<Drop> _drops = new();
    
    // Flash pixels (sudden white pop that fades to green)
    private readonly Dictionary<int, FlashState> _flashes = new();

    private record Drop(double SpawnTime, double Speed);
    private record FlashState(int FramesAlive, int TotalFrames, double PeakBrightness);

    public MatrixRainRenderer(double maxBrightness, double dropChance, double flashChance, int trailLength)
    {
        _maxBrightness = maxBrightness;
        _dropChance = dropChance;
        _flashChance = flashChance;
        _trailLength = trailLength;
        
        _phases = new double[PixelCount];
        _speeds = new double[PixelCount];
        _targetBrightness = new double[PixelCount];
        _currentBrightness = new double[PixelCount];
        
        InitializePixelStates();
    }

    public static MatrixRainRenderer Create(JsonElement json)
    {
        var maxBrightness = json.TryGetProperty("max_brightness", out var mb) ? mb.GetDouble() : 0.12;
        var dropChance = json.TryGetProperty("drop_chance", out var dc) ? dc.GetDouble() : 0.02;
        var flashChance = json.TryGetProperty("flash_chance", out var fc) ? fc.GetDouble() : 0.008;
        var trailLength = json.TryGetProperty("trail_length", out var tl) ? tl.GetInt32() : 4;
        
        return new MatrixRainRenderer(maxBrightness, dropChance, flashChance, trailLength);
    }

    private void InitializePixelStates()
    {
        for (int i = 0; i < PixelCount; i++)
        {
            // Each pixel has its own random phase and speed for shimmer
            _phases[i] = Random.NextDouble() * Math.PI * 2;
            _speeds[i] = 0.02 + Random.NextDouble() * 0.06; // Variable speeds
            _targetBrightness[i] = Random.NextDouble() * _maxBrightness * 0.5;
            _currentBrightness[i] = _targetBrightness[i];
        }
    }

    public override void Render(BlinktController controller, Color color, double elapsedSeconds)
    {
        // Update state
        UpdateBackgroundShimmer();
        UpdateDrops(elapsedSeconds);
        UpdateFlashes();
        SpawnNewEffects(elapsedSeconds);
        
        // Render to pixels
        RenderToPixels(controller, elapsedSeconds);
        controller.Show();
    }

    private void UpdateBackgroundShimmer()
    {
        for (int i = 0; i < PixelCount; i++)
        {
            // Advance phase
            _phases[i] += _speeds[i];
            
            // Occasionally pick a new random target brightness (creates organic variance)
            if (Random.NextDouble() < 0.02)
            {
                _targetBrightness[i] = _maxBrightness * (0.1 + Random.NextDouble() * 0.4);
            }
            
            // Smooth towards target (creates gentle transitions)
            var diff = _targetBrightness[i] - _currentBrightness[i];
            _currentBrightness[i] += diff * 0.1;
            
            // Add sine-wave oscillation
            var sineOffset = Math.Sin(_phases[i]) * _maxBrightness * 0.15;
            _currentBrightness[i] = Math.Clamp(_currentBrightness[i] + sineOffset, 0, _maxBrightness * 0.6);
        }
    }

    private void UpdateDrops(double elapsedSeconds)
    {
        // Remove drops that have traveled off screen
        _drops.RemoveAll(d => 
        {
            var age = elapsedSeconds - d.SpawnTime;
            var pos = age * d.Speed;
            return pos > PixelCount + _trailLength + 2;
        });
    }

    private void UpdateFlashes()
    {
        var toRemove = new List<int>();
        foreach (var kvp in _flashes)
        {
            var state = kvp.Value;
            if (state.FramesAlive >= state.TotalFrames)
            {
                toRemove.Add(kvp.Key);
            }
            else
            {
                _flashes[kvp.Key] = state with { FramesAlive = state.FramesAlive + 1 };
            }
        }
        foreach (var key in toRemove)
        {
            _flashes.Remove(key);
        }
    }

    private void SpawnNewEffects(double elapsedSeconds)
    {
        // Spawn new drops (bright white/green traveling across)
        if (Random.NextDouble() < _dropChance && _drops.Count < 2)
        {
            var speed = 2.0 + Random.NextDouble() * 3.0; // Variable speeds
            _drops.Add(new Drop(elapsedSeconds, speed));
        }
        
        // Spawn flashes (sudden bright pop on a pixel)
        if (Random.NextDouble() < _flashChance)
        {
            var pixel = Random.Next(PixelCount);
            if (!_flashes.ContainsKey(pixel))
            {
                var frames = 15 + Random.Next(25); // 15-40 frames fade
                var peak = _maxBrightness * (0.8 + Random.NextDouble() * 0.2);
                _flashes[pixel] = new FlashState(0, frames, peak);
            }
        }
    }

    private void RenderToPixels(BlinktController controller, double elapsedSeconds)
    {
        for (int i = 0; i < PixelCount; i++)
        {
            // Start with background shimmer
            var brightness = _currentBrightness[i];
            var whiteness = 0.0; // 0 = pure green, 1 = pure white
            
            // Check if any drop affects this pixel
            foreach (var drop in _drops)
            {
                var age = elapsedSeconds - drop.SpawnTime;
                var headPos = age * drop.Speed;
                
                // Is this pixel in the drop's trail?
                var pixelOffset = headPos - i;
                if (pixelOffset >= 0 && pixelOffset < _trailLength)
                {
                    // How far back in the trail? (0 = head, 1 = tail)
                    var trailProgress = pixelOffset / _trailLength;
                    
                    // Head is bright white, fades to green along trail
                    var dropBrightness = _maxBrightness * (1.0 - trailProgress * 0.7);
                    
                    // Head is whiter, trail gets progressively greener
                    var dropWhiteness = Math.Max(0, 1.0 - trailProgress * 1.5);
                    
                    // Additive blend with existing
                    brightness = Math.Max(brightness, dropBrightness);
                    whiteness = Math.Max(whiteness, dropWhiteness);
                }
            }
            
            // Check for flash effect on this pixel
            if (_flashes.TryGetValue(i, out var flash))
            {
                var flashProgress = (double)flash.FramesAlive / flash.TotalFrames;
                var flashBrightness = flash.PeakBrightness * (1.0 - flashProgress);
                var flashWhiteness = Math.Max(0, 1.0 - flashProgress * 2); // Goes green faster
                
                brightness = Math.Max(brightness, flashBrightness);
                whiteness = Math.Max(whiteness, flashWhiteness);
            }
            
            // Convert to color - blend between green and white based on whiteness
            var green = Color.FromArgb(0, 255, 0);
            var white = Color.FromArgb(255, 255, 255);
            
            var r = (byte)(green.R + (white.R - green.R) * whiteness);
            var g = (byte)(green.G + (white.G - green.G) * whiteness); // Stays 255
            var b = (byte)(green.B + (white.B - green.B) * whiteness);
            
            var finalColor = Color.FromArgb(r, g, b);
            SetPixelSmooth(controller, i, finalColor, brightness);
        }
    }
}
