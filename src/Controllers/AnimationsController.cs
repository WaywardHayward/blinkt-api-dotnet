using Microsoft.AspNetCore.Mvc;
using BlinktApi.Services;
using BlinktApi.Models;
using System.Text.Json;

namespace BlinktApi.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class AnimationsController : ControllerBase
{
    private readonly AnimationController _controller;
    private readonly ILogger<AnimationsController> _logger;
    private readonly string _animationsPath;

    public AnimationsController(AnimationController controller, ILogger<AnimationsController> logger)
    {
        _controller = controller;
        _logger = logger;
        _animationsPath = Path.Combine(AppContext.BaseDirectory, "animations");
    }

    /// <summary>
    /// List all available animations
    /// </summary>
    /// <returns>Array of animation names</returns>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var animations = _controller.GetAnimationNames();
        return Ok(new { animations });
    }

    /// <summary>
    /// Get details for a specific animation
    /// </summary>
    /// <param name="name">Animation name</param>
    /// <returns>Animation definition with type, description, and parameters</returns>
    [HttpGet("{name}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get(string name)
    {
        var animation = _controller.GetAnimation(name);
        if (animation == null)
            return NotFound(new { error = $"Animation '{name}' not found" });
            
        return Ok(new { animation });
    }

    /// <summary>
    /// Upload a new animation or update an existing one
    /// </summary>
    /// <param name="animation">Animation definition JSON</param>
    /// <returns>Upload confirmation</returns>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public IActionResult Upload([FromBody] Animation animation)
    {
        if (string.IsNullOrWhiteSpace(animation.Name))
        {
            return BadRequest(new { error = "Animation name is required" });
        }

        // Validate animation name (alphanumeric + hyphens only)
        if (!System.Text.RegularExpressions.Regex.IsMatch(animation.Name, @"^[a-zA-Z0-9\-]+$"))
        {
            return BadRequest(new { error = "Animation name can only contain letters, numbers, and hyphens" });
        }

        try
        {
            // Ensure animations directory exists
            Directory.CreateDirectory(_animationsPath);

            // Write to file
            var filePath = Path.Combine(_animationsPath, $"{animation.Name}.json");
            var json = JsonSerializer.Serialize(animation, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            var isUpdate = System.IO.File.Exists(filePath);
            System.IO.File.WriteAllText(filePath, json);

            _logger.LogInformation("{Action} animation: {Name} to {Path}", 
                isUpdate ? "Updated" : "Created", 
                animation.Name, 
                filePath);

            // Hot reload will pick this up, but we can also reload immediately
            _controller.LoadAnimations(_animationsPath);

            return Created($"/animations/{animation.Name}", new
            {
                status = "ok",
                action = isUpdate ? "updated" : "created",
                name = animation.Name,
                path = filePath
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload animation: {Name}", animation.Name);
            return StatusCode(500, new { error = "Failed to save animation", details = ex.Message });
        }
    }

    /// <summary>
    /// Delete an animation
    /// </summary>
    /// <param name="name">Animation name to delete</param>
    /// <returns>Deletion confirmation</returns>
    [HttpDelete("{name}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(string name)
    {
        var filePath = Path.Combine(_animationsPath, $"{name}.json");

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(new { error = $"Animation '{name}' not found" });
        }

        try
        {
            System.IO.File.Delete(filePath);
            _logger.LogInformation("Deleted animation: {Name} from {Path}", name, filePath);

            // Reload animations
            _controller.LoadAnimations(_animationsPath);

            return Ok(new
            {
                status = "ok",
                action = "deleted",
                name = name
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete animation: {Name}", name);
            return StatusCode(500, new { error = "Failed to delete animation", details = ex.Message });
        }
    }
}
