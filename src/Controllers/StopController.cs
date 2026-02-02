using Microsoft.AspNetCore.Mvc;
using BlinktApi.Services;

namespace BlinktApi.Controllers;

[ApiController]
[Route("[controller]")]
public class StopController : ControllerBase
{
    private readonly AnimationController _controller;
    private readonly ILogger<StopController> _logger;

    public StopController(AnimationController controller, ILogger<StopController> logger)
    {
        _controller = controller;
        _logger = logger;
    }

    /// <summary>
    /// Stop the currently running animation
    /// </summary>
    [HttpPost]
    public IActionResult Post()
    {
        _logger.LogInformation("Stop requested");
        _controller.StopAnimation();
        
        return Ok(new { status = "stopped" });
    }
}
