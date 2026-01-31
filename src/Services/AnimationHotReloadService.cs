namespace BlinktApi.Services;

public class AnimationHotReloadService : BackgroundService
{
    private readonly AnimationController _controller;
    private readonly ILogger<AnimationHotReloadService> _logger;
    private FileSystemWatcher? _watcher;
    private readonly string _animationsPath;

    public AnimationHotReloadService(AnimationController controller, ILogger<AnimationHotReloadService> logger)
    {
        _controller = controller;
        _logger = logger;
        _animationsPath = Path.Combine(AppContext.BaseDirectory, "animations");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!Directory.Exists(_animationsPath))
        {
            _logger.LogWarning("Animations directory not found: {Path}. Hot reload disabled.", _animationsPath);
            return Task.CompletedTask;
        }

        _logger.LogInformation("Starting animation hot reload watcher on {Path}", _animationsPath);

        _watcher = new FileSystemWatcher(_animationsPath)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime,
            Filter = "*.json",
            EnableRaisingEvents = true
        };

        _watcher.Changed += OnAnimationFileChanged;
        _watcher.Created += OnAnimationFileChanged;
        _watcher.Deleted += OnAnimationFileChanged;
        _watcher.Renamed += OnAnimationFileRenamed;

        _logger.LogInformation("Animation hot reload enabled - watching for changes");

        return Task.CompletedTask;
    }

    private void OnAnimationFileChanged(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("Animation file {ChangeType}: {FileName}", e.ChangeType, Path.GetFileName(e.FullPath));
        
        // Debounce - wait a bit for file writes to complete
        Task.Delay(500).ContinueWith(_ =>
        {
            try
            {
                _logger.LogInformation("Reloading animations...");
                _controller.LoadAnimations(_animationsPath);
                _logger.LogInformation("Hot reload complete");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reload animations");
            }
        });
    }

    private void OnAnimationFileRenamed(object sender, RenamedEventArgs e)
    {
        _logger.LogInformation("Animation file renamed: {OldName} -> {NewName}", 
            Path.GetFileName(e.OldFullPath), 
            Path.GetFileName(e.FullPath));
        
        Task.Delay(500).ContinueWith(_ =>
        {
            try
            {
                _logger.LogInformation("Reloading animations...");
                _controller.LoadAnimations(_animationsPath);
                _logger.LogInformation("Hot reload complete");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reload animations");
            }
        });
    }

    public override void Dispose()
    {
        _watcher?.Dispose();
        base.Dispose();
    }
}
