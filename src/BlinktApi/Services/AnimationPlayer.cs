using BlinktApi.Hardware;
using BlinktApi.Models;

namespace BlinktApi.Services;

public class AnimationPlayer : BackgroundService
{
    private readonly AnimationController _controller;
    private readonly ILogger<AnimationPlayer> _logger;
    private readonly BlinktController _blinkt;

    public AnimationPlayer(AnimationController controller, ILogger<AnimationPlayer> logger)
    {
        _controller = controller;
        _logger = logger;
        _blinkt = new BlinktController();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Animation player started");
        
        // Load animations from disk
        var animationsPath = Path.Combine(AppContext.BaseDirectory, "animations");
        if (Directory.Exists(animationsPath))
        {
            _controller.LoadAnimations(animationsPath);
            _logger.LogInformation("Loaded {Count} animations", _controller.GetAnimationNames().Count());
        }

        // Test the hardware
        _logger.LogInformation("Testing Blinkt hardware...");
        await TestBlinkt();

        // Main animation loop
        var frameTime = TimeSpan.FromMilliseconds(20); // ~50 FPS
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var frameStart = DateTime.UtcNow;
            
            // Check for expired animations
            _controller.CheckExpiration();
            
            // Render current animation
            var (state, renderer) = _controller.GetCurrent();
            if (state != null && renderer != null)
            {
                var elapsed = (DateTime.UtcNow - state.StartTime).TotalSeconds;
                try
                {
                    renderer.Render(_blinkt, state.Color, elapsed);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Animation rendering error");
                }
            }
            else
            {
                // No animation - clear LEDs
                _blinkt.Clear();
                _blinkt.Show();
            }
            
            // Frame timing
            var frameElapsed = DateTime.UtcNow - frameStart;
            var delay = frameTime - frameElapsed;
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay, stoppingToken);
            }
        }
        
        _logger.LogInformation("Animation player stopped");
    }

    private async Task TestBlinkt()
    {
        try
        {
            // Quick test: Flash all LEDs green
            _blinkt.SetAll(0, 255, 0, 0.1);
            _blinkt.Show();
            await Task.Delay(500);
            _blinkt.Clear();
            _blinkt.Show();
            _logger.LogInformation("Blinkt hardware test complete");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Blinkt hardware test failed - running in test mode");
        }
    }

    public override void Dispose()
    {
        _blinkt.Dispose();
        base.Dispose();
    }
}
