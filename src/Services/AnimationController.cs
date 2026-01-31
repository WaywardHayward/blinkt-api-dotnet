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

    public AnimationController(ILogger<AnimationController> logger)
    {
        _logger = logger;
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
            try
            {
                var json = File.ReadAllText(file);
                var animation = JsonSerializer.Deserialize<Animation>(json);
                
                if (animation?.Name != null)
                {
                    _animations[animation.Name] = animation;
                    loadedCount++;
                    _logger.LogDebug("Loaded animation: {Name} from {File}", animation.Name, Path.GetFileName(file));
                }
                else
                {
                    _logger.LogWarning("Animation file {File} has no name property", Path.GetFileName(file));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load animation from {File}", Path.GetFileName(file));
            }
        }
        
        _logger.LogInformation("Successfully loaded {LoadedCount}/{TotalCount} animations", loadedCount, files.Length);
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

            var renderer = RendererFactory.CreateRenderer(animation);
            
            if (renderer == null)
            {
                _logger.LogError("Failed to create renderer for animation: {Name}", name);
                throw new InvalidOperationException($"Could not create renderer for animation '{name}'");
            }

            // If there's a current animation with finite duration, push to stack
            if (_currentState != null && _currentState.DurationSeconds > 0)
            {
                _stack.Push(_currentState);
                _logger.LogDebug("Pushed animation {Name} to stack (depth: {Depth})", _currentState.Name, _stack.Count);
            }

            _currentState = new AnimationState
            {
                Name = name,
                Color = color,
                StartTime = DateTime.UtcNow,
                DurationSeconds = durationSeconds
            };
            _currentRenderer = renderer;
            
            _logger.LogInformation(
                "Started animation: {Name}, Color: {Color}, Duration: {Duration}s", 
                name, 
                $"#{color.R:X2}{color.G:X2}{color.B:X2}", 
                durationSeconds > 0 ? durationSeconds.ToString() : "infinite");
        }
    }

    public void StopAnimation()
    {
        lock (_lock)
        {
            if (_stack.Count > 0)
            {
                // Pop back to previous animation
                var previous = _stack.Pop();
                var animation = GetAnimation(previous.Name);
                if (animation != null)
                {
                    _currentState = previous;
                    _currentRenderer = RendererFactory.CreateRenderer(animation);
                    _logger.LogInformation("Popped back to animation: {Name} (stack depth: {Depth})", previous.Name, _stack.Count);
                }
            }
            else
            {
                _logger.LogInformation("Stopped animation: {Name}", _currentState?.Name ?? "(none)");
                _currentState = null;
                _currentRenderer = null;
            }
        }
    }

    public void CheckExpiration()
    {
        lock (_lock)
        {
            if (_currentState?.IsExpired == true)
            {
                _logger.LogDebug("Animation {Name} expired after {Duration}s", _currentState.Name, _currentState.DurationSeconds);
                
                // Check if there's a queued animation
                if (_queue.Count > 0)
                {
                    var next = _queue.Dequeue();
                    _logger.LogInformation("Playing next queued animation: {Name} ({QueueRemaining} remaining)", next.Name, _queue.Count);
                    StartAnimation(next.Name, next.Color, next.DurationSeconds);
                }
                else
                {
                    StopAnimation();
                }
            }
        }
    }
    
    public void EnqueueAnimations(IEnumerable<QueuedAnimation> animations)
    {
        lock (_lock)
        {
            foreach (var anim in animations)
            {
                _queue.Enqueue(anim);
            }
            _logger.LogInformation("Enqueued {Count} animations (total queue: {Total})", animations.Count(), _queue.Count);
            
            // If nothing is playing, start the first one
            if (_currentState == null && _queue.Count > 0)
            {
                var first = _queue.Dequeue();
                _logger.LogInformation("Starting first queued animation: {Name}", first.Name);
                StartAnimation(first.Name, first.Color, first.DurationSeconds);
            }
        }
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
