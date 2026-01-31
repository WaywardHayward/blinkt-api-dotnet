using System.Collections.Concurrent;
using BlinktApi.Models;

namespace BlinktApi.Services;

public class AnimationController
{
    private readonly ConcurrentDictionary<string, Animation> _animations = new();
    private string? _currentAnimation;
    private string? _currentColor;
    private bool _isRunning;
    private readonly Stack<(string Name, string Color, int Duration)> _stack = new();

    public void LoadAnimations(string animationsPath)
    {
        // TODO: Load animation JSON files from disk
        // For now, just add a test animation
        _animations["pulse"] = new Animation
        {
            Type = "global_oscillator",
            Name = "pulse",
            Description = "All LEDs pulse together",
            Author = "Rabbie Pi",
            Version = "1.0",
            Parameters = new Dictionary<string, object>
            {
                ["brightness_range"] = new[] { 0.02, 0.08 },
                ["fps"] = 100,
                ["oscillator"] = "sine",
                ["period_seconds"] = 3
            }
        };
    }

    public IEnumerable<string> GetAnimationNames() => _animations.Keys;

    public Animation? GetAnimation(string name) =>
        _animations.TryGetValue(name, out var animation) ? animation : null;

    public void StartAnimation(string name, string color, int duration)
    {
        _currentAnimation = name;
        _currentColor = color;
        _isRunning = true;
        
        // TODO: Implement stacking logic
    }

    public void StopAnimation()
    {
        _isRunning = false;
    }

    public object GetStatus() => new
    {
        current_animation = _currentAnimation,
        current_color = _currentColor,
        is_running = _isRunning,
        animation_count = _animations.Count,
        stack_depth = _stack.Count,
        stack = _stack.Select(s => $"{s.Name} ({s.Color})").ToArray()
    };
}
