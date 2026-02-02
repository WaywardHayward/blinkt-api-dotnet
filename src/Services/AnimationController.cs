using System.Collections.Concurrent;
using System.Drawing;
using System.Text.Json;
using BlinktApi.Models;
using BlinktApi.Rendering;
using Microsoft.Extensions.Logging;

namespace BlinktApi.Services;

public class AnimationController
{
    private readonly ConcurrentDictionary<string, Animation> _animations = new();
    private readonly Stack<AnimationState> _stack = new();
    private readonly Queue<QueuedAnimation> _queue = new();
    private AnimationState? _currentState;
    private IAnimationRenderer? _currentRenderer;
    private readonly object _lock = new();
    private readonly ILogger<AnimationController> _logger;
    private readonly RendererFactory _rendererFactory;

    public AnimationController(ILogger<AnimationController> logger, RendererFactory rendererFactory)
    {
        _logger = logger;
        _rendererFactory = rendererFactory;
    }

    public void LoadAnimations(string animationsPath)
    {
        if (!Directory.Exists(animationsPath))
        {
            _logger.LogWarning("Animations directory not found: {Path}", animationsPath);
            return;
        }

        var files = Directory.GetFiles(animationsPath, "*.json");
        _logger.LogInformation("Loading animations from {Path} ({Count} files)", animationsPath, files.Length);

        var loadedCount = 0;
        foreach (var file in files)
        {
            if (TryLoadAnimationFile(file))
                loadedCount++;
        }
        
        _logger.LogInformation("Successfully loaded {LoadedCount}/{TotalCount} animations", loadedCount, files.Length);
    }

    private bool TryLoadAnimationFile(string file)
    {
        try
        {
            var json = File.ReadAllText(file);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var animation = JsonSerializer.Deserialize<Animation>(json, options);
            
            if (animation?.Name == null)
            {
                _logger.LogWarning("Animation file {File} has no name property", Path.GetFileName(file));
                return false;
            }

            _animations[animation.Name] = animation;
            _logger.LogDebug("Loaded animation: {Name} from {File}", animation.Name, Path.GetFileName(file));
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load animation from {File}", Path.GetFileName(file));
            return false;
        }
    }

    public IEnumerable<string> GetAnimationNames() => _animations.Keys.OrderBy(k => k);

    public Animation? GetAnimation(string name) =>
        _animations.TryGetValue(name, out var animation) ? animation : null;

    public void StartAnimation(string name, Color color, int durationSeconds)
    {
        lock (_lock)
        {
            var animation = GetAnimation(name);
            if (animation == null)
            {
                _logger.LogWarning("Attempt to start unknown animation: {Name}", name);
                throw new KeyNotFoundException($"Animation '{name}' not found");
            }

            var renderer = _rendererFactory.CreateRenderer(animation);
            if (renderer == null)
            {
                _logger.LogError("Failed to create renderer for animation: {Name}", name);
                throw new InvalidOperationException($"Could not create renderer for animation '{name}'");
            }

            PushCurrentToStackIfFinite();
            SetCurrentAnimation(name, color, durationSeconds, renderer);
        }
    }

    private void PushCurrentToStackIfFinite()
    {
        if (_currentState == null || _currentState.DurationSeconds <= 0)
            return;

        _stack.Push(_currentState);
        _logger.LogDebug("Pushed animation {Name} to stack (depth: {Depth})", _currentState.Name, _stack.Count);
    }

    private void SetCurrentAnimation(string name, Color color, int durationSeconds, IAnimationRenderer renderer)
    {
        _currentState = new AnimationState
        {
            Name = name,
            Color = color,
            StartTime = DateTime.UtcNow,
            DurationSeconds = durationSeconds
        };
        _currentRenderer = renderer;
        
        var durationText = durationSeconds > 0 ? durationSeconds.ToString() : "infinite";
        _logger.LogInformation(
            "Started animation: {Name}, Color: {Color}, Duration: {Duration}s", 
            name, 
            $"#{color.R:X2}{color.G:X2}{color.B:X2}", 
            durationText);
    }

    public void StopAnimation()
    {
        lock (_lock)
        {
            if (!TryPopPreviousAnimation())
                ClearCurrentAnimation();
        }
    }

    private bool TryPopPreviousAnimation()
    {
        if (_stack.Count == 0)
            return false;

        var previous = _stack.Pop();
        var animation = GetAnimation(previous.Name);
        
        if (animation == null)
            return false;

        _currentState = previous;
        _currentRenderer = _rendererFactory.CreateRenderer(animation);
        _logger.LogInformation("Popped back to animation: {Name} (stack depth: {Depth})", previous.Name, _stack.Count);
        return true;
    }

    private void ClearCurrentAnimation()
    {
        _logger.LogInformation("Stopped animation: {Name}", _currentState?.Name ?? "(none)");
        _currentState = null;
        _currentRenderer = null;
    }

    public void CheckExpiration()
    {
        lock (_lock)
        {
            if (_currentState?.IsExpired != true)
                return;

            _logger.LogDebug("Animation {Name} expired after {Duration}s", _currentState.Name, _currentState.DurationSeconds);
            
            if (TryPlayNextQueuedAnimation())
                return;

            StopAnimation();
        }
    }

    private bool TryPlayNextQueuedAnimation()
    {
        if (_queue.Count == 0)
            return false;

        var next = _queue.Dequeue();
        _logger.LogInformation("Playing next queued animation: {Name} ({QueueRemaining} remaining)", next.Name, _queue.Count);
        StartAnimation(next.Name, next.Color, next.DurationSeconds);
        return true;
    }
    
    public void EnqueueAnimations(IEnumerable<QueuedAnimation> animations)
    {
        lock (_lock)
        {
            var animList = animations.ToList();
            foreach (var anim in animList)
            {
                _queue.Enqueue(anim);
            }
            
            _logger.LogInformation("Enqueued {Count} animations (total queue: {Total})", animList.Count, _queue.Count);
            
            TryStartFirstQueuedAnimation();
        }
    }

    private void TryStartFirstQueuedAnimation()
    {
        if (_currentState != null || _queue.Count == 0)
            return;

        var first = _queue.Dequeue();
        _logger.LogInformation("Starting first queued animation: {Name}", first.Name);
        StartAnimation(first.Name, first.Color, first.DurationSeconds);
    }
    
    public void ClearQueue()
    {
        lock (_lock)
        {
            var count = _queue.Count;
            _queue.Clear();
            _logger.LogInformation("Cleared animation queue ({Count} items removed)", count);
        }
    }
    
    public int GetQueueLength()
    {
        lock (_lock)
        {
            return _queue.Count;
        }
    }

    public (AnimationState? state, IAnimationRenderer? renderer) GetCurrent()
    {
        lock (_lock)
        {
            return (_currentState, _currentRenderer);
        }
    }

    public object GetStatus() => new
    {
        current_animation = _currentState?.Name,
        current_color = _currentState?.Color.Name,
        is_running = _currentState != null,
        animation_count = _animations.Count,
        stack_depth = _stack.Count,
        queue_length = _queue.Count,
        stack = _stack.Select(s => $"{s.Name} ({s.Color.Name})").ToArray(),
        queue = _queue.Select(q => $"{q.Name} ({q.DurationSeconds}s)").ToArray()
    };
}
