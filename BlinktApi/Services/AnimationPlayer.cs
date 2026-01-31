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
        }

        // Test the hardware
        _logger.LogInformation("Testing Blinkt hardware...");
        TestBlinkt();

        // Main animation loop
        while (!stoppingToken.IsCancellationRequested)
        {
            // TODO: Implement animation rendering loop
            // For now, just idle
            await Task.Delay(100, stoppingToken);
        }
        
        _logger.LogInformation("Animation player stopped");
    }

    private void TestBlinkt()
    {
        try
        {
            // Quick test: Flash all LEDs green
            _blinkt.SetAll(0, 255, 0, 0.1);
            _blinkt.Show();
            Thread.Sleep(500);
            _blinkt.Clear();
            _blinkt.Show();
            _logger.LogInformation("Blinkt hardware test complete");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Blinkt hardware test failed");
        }
    }

    public override void Dispose()
    {
        _blinkt.Dispose();
        base.Dispose();
    }
}
