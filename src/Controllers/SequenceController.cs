using System.Drawing;
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
    /// <param name="request">Animation request with name, color (string/hex or rgb object), and optional duration</param>
    /// <returns>Animation started confirmation</returns>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public IActionResult Post([FromBody] AnimationRequest request)
    {
        var (color, colorDescription) = ParseColor(request);
        LogAnimationStart(request.Name, colorDescription, request.Duration);
        
        _controller.StartAnimation(request.Name, color, request.Duration);
        
        return Ok(new
        {
            status = "ok",
            sequence = request.Name,
            color = colorDescription,
            duration = request.Duration
        });
    }

    private (Color color, string description) ParseColor(AnimationRequest request)
    {
        if (request.Rgb != null)
            return ParseRgbColor(request.Rgb);

        return ParseStringColor(request.ColorString ?? "blue");
    }

    private (Color color, string description) ParseRgbColor(RgbColor rgb)
    {
        var color = ColorHelper.FromRgb(rgb.R, rgb.G, rgb.B);
        var description = $"rgb({rgb.R},{rgb.G},{rgb.B})";
        return (color, description);
    }

    private (Color color, string description) ParseStringColor(string colorString)
    {
        var color = ColorHelper.Parse(colorString);
        return (color, colorString);
    }

    private void LogAnimationStart(string name, string colorDescription, int duration)
    {
        _logger.LogInformation(
            "Starting animation: {Name}, Color: {Color}, Duration: {Duration}s", 
            name, 
            colorDescription, 
            duration);
    }
}
