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
