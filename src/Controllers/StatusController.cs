using Microsoft.AspNetCore.Mvc;
using BlinktApi.Services;

namespace BlinktApi.Controllers;

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
