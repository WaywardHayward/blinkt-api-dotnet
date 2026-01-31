using BlinktApi.Models;

namespace BlinktApi.Services;

public class AnimationPlayer : BackgroundService
{
    private readonly AnimationController _controller;
    private readonly ILogger<AnimationPlayer> _logger;

    public AnimationPlayer(AnimationController controller, ILogger<AnimationPlayer> logger)
    {
        _controller = controller;
        _logger = logger;
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

        // Main animation loop
        while (!stoppingToken.IsCancellationRequested)
        {
            // TODO: Implement animation rendering loop
            // For now, just idle
            await Task.Delay(100, stoppingToken);
        }
        
        _logger.LogInformation("Animation player stopped");
    }
}
