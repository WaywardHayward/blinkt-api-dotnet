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
    private readonly ILogger<SequenceController> _logger;

    public SequenceController(AnimationController controller, ILogger<SequenceController> logger)
    {
        _controller = controller;
        _logger = logger;
    }

    /// <summary>
    /// Start an animation sequence
    /// </summary>
    /// <param name="request">Animation request with name, color, and optional duration</param>
    /// <returns>Animation started confirmation</returns>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public IActionResult Post([FromBody] AnimationRequest request)
    {
        _logger.LogInformation("Starting animation: {Name}, Color: {Color}, Duration: {Duration}s", 
            request.Name, request.Color, request.Duration);
        
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
