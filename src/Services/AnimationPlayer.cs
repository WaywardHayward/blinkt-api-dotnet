using BlinktApi.Hardware;
using BlinktApi.Models;
using BlinktApi.Rendering;

namespace BlinktApi.Services;

public class AnimationPlayer : BackgroundService
{
    private readonly AnimationController _controller;
    private readonly ILogger<AnimationPlayer> _logger;
    private readonly BlinktController _blinkt;
    private static readonly TimeSpan FrameTime = TimeSpan.FromMilliseconds(16); // ~60 FPS

    public AnimationPlayer(AnimationController controller, ILogger<AnimationPlayer> logger, ILoggerFactory loggerFactory)
    {
        _controller = controller;
        _logger = logger;
        _blinkt = new BlinktController(loggerFactory.CreateLogger<BlinktController>());
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Animation player started");
        
        await InitializeAsync();
        await RunAnimationLoopAsync(stoppingToken);
        
        _logger.LogInformation("Animation player stopped");
    }

    private async Task InitializeAsync()
    {
        LoadAnimations();
        await TestBlinktAsync();
    }

    private void LoadAnimations()
    {
        var animationsPath = Path.Combine(AppContext.BaseDirectory, "animations");
        
        if (!Directory.Exists(animationsPath))
            return;

        _controller.LoadAnimations(animationsPath);
        _logger.LogInformation("Loaded {Count} animations", _controller.GetAnimationNames().Count());
    }

    private async Task RunAnimationLoopAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var frameStart = DateTime.UtcNow;
            
            ProcessFrame();
            
            await DelayForFrameTimingAsync(frameStart, stoppingToken);
        }
    }

    private void ProcessFrame()
    {
        _controller.CheckExpiration();
        
        var (state, renderer) = _controller.GetCurrent();
        
        if (HasActiveAnimation(state, renderer))
            RenderAnimation(state!, renderer!);
        else
            ClearDisplay();
    }

    private bool HasActiveAnimation(AnimationState? state, IAnimationRenderer? renderer) =>
        state != null && renderer != null;

    private void RenderAnimation(AnimationState state, IAnimationRenderer renderer)
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

    private void ClearDisplay()
    {
        _blinkt.Clear();
        _blinkt.Show();
    }

    private async Task DelayForFrameTimingAsync(DateTime frameStart, CancellationToken stoppingToken)
    {
        var frameElapsed = DateTime.UtcNow - frameStart;
        var delay = FrameTime - frameElapsed;
        
        if (delay > TimeSpan.Zero)
            await Task.Delay(delay, stoppingToken);
    }

    private async Task TestBlinktAsync()
    {
        _logger.LogInformation("Testing Blinkt hardware...");
        
        try
        {
            await FlashTestPatternAsync();
            _logger.LogInformation("Blinkt hardware test complete");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Blinkt hardware test failed - running in test mode");
        }
    }

    private async Task FlashTestPatternAsync()
    {
        _blinkt.SetAll(0, 255, 0, 0.1);
        _blinkt.Show();
        await Task.Delay(500);
        _blinkt.Clear();
        _blinkt.Show();
    }

    public override void Dispose()
    {
        _blinkt.Dispose();
        base.Dispose();
    }
}
