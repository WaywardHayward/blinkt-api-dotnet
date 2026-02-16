using Microsoft.AspNetCore.Mvc;
using BlinktApi.Services;

namespace BlinktApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ClearController : ControllerBase
{
    private readonly AnimationController _controller;
    private readonly ILogger<ClearController> _logger;

    public ClearController(AnimationController controller, ILogger<ClearController> logger)
    {
        _controller = controller;
        _logger = logger;
    }

    /// <summary>
    /// Clear all LEDs and stop any running animation
    /// </summary>
    [HttpPost]
    public IActionResult Post()
    {
        _logger.LogInformation("Clear requested - stopping animation");
        _controller.StopAnimation();
        
        return Ok(new { status = "cleared" });
    }
}
