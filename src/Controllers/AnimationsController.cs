using Microsoft.AspNetCore.Mvc;
using BlinktApi.Services;

namespace BlinktApi.Controllers;

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
