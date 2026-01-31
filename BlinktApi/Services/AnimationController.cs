using System.Collections.Concurrent;
using System.Text.Json;
using BlinktApi.Models;
using BlinktApi.Rendering;

namespace BlinktApi.Services;

public class AnimationController
{
    private readonly ConcurrentDictionary<string, Animation> _animations = new();
    private readonly Stack<AnimationState> _stack = new();
    private AnimationState? _currentState;
    private IAnimationRenderer? _currentRenderer;
    private readonly object _lock = new();

    public void LoadAnimations(string animationsPath)
    {
        if (!Directory.Exists(animationsPath))
            return;

        foreach (var file in Directory.GetFiles(animationsPath, "*.json"))
        {
            try
            {
                var json = File.ReadAllText(file);
                var animation = JsonSerializer.Deserialize<Animation>(json);
                
                if (animation?.Name != null)
                {
                    _animations[animation.Name] = animation;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load animation {file}: {ex.Message}");
            }
        }
    }

    public IEnumerable<string> GetAnimationNames() => _animations.Keys.OrderBy(k => k);

    public Animation? GetAnimation(string name) =>
        _animations.TryGetValue(name, out var animation) ? animation : null;

    public void StartAnimation(string name, string colorName, int durationSeconds)
    {
        lock (_lock)
        {
            var animation = GetAnimation(name);
            if (animation == null)
                return;

            var color = ColorHelper.Parse(colorName);
            var renderer = RendererFactory.CreateRenderer(animation);
            
            if (renderer == null)
                return;

            // If there's a current animation with finite duration, push to stack
            if (_currentState != null && _currentState.DurationSeconds > 0)
            {
                _stack.Push(_currentState);
            }

            _currentState = new AnimationState
            {
                Name = name,
                Color = color,
                StartTime = DateTime.UtcNow,
                DurationSeconds = durationSeconds
            };
            _currentRenderer = renderer;
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
                }
            }
            else
            {
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
                StopAnimation();
            }
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
        stack = _stack.Select(s => $"{s.Name} ({s.Color.Name})").ToArray()
    };
}
