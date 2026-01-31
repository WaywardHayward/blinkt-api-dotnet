using Microsoft.AspNetCore.Mvc;
using BlinktApi.Models;
using BlinktApi.Services;

namespace BlinktApi.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class SequenceController : ControllerBase
{
    private readonly AnimationController _controller;

    public SequenceController(AnimationController controller)
    {
        _controller = controller;
    }

    /// <summary>
    /// Start an animation sequence
    /// </summary>
    /// <param name="request">Animation request with name, color, and optional duration</param>
    /// <returns>Animation started confirmation</returns>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult Post([FromBody] AnimationRequest request)
    {
        _controller.StartAnimation(request.Name, request.Color, request.Duration);
        
        return Ok(new
        {
            status = "ok",
            sequence = request.Name,
            color = request.Color,
            duration = request.Duration
        });
    }
}

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class StatusController : ControllerBase
{
    private readonly AnimationController _controller;

    public StatusController(AnimationController controller)
    {
        _controller = controller;
    }

    /// <summary>
    /// Get current animation status
    /// </summary>
    /// <returns>Current animation state, stack depth, and animation count</returns>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(_controller.GetStatus());
    }
}

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class AnimationsController : ControllerBase
{
    private readonly AnimationController _controller;

    public AnimationsController(AnimationController controller)
    {
        _controller = controller;
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
            return NotFound();
            
        return Ok(new { animation });
    }
}
