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
        // Determine color from either string or RGB
        Color color;
        string colorDescription;
        
        if (request.Rgb != null)
        {
            color = ColorHelper.FromRgb(request.Rgb.R, request.Rgb.G, request.Rgb.B);
            colorDescription = $"rgb({request.Rgb.R},{request.Rgb.G},{request.Rgb.B})";
            _logger.LogInformation("Starting animation: {Name}, RGB: ({R},{G},{B}), Duration: {Duration}s", 
                request.Name, request.Rgb.R, request.Rgb.G, request.Rgb.B, request.Duration);
        }
        else
        {
            var colorString = request.ColorString ?? "blue";
            color = ColorHelper.Parse(colorString);
            colorDescription = colorString;
            _logger.LogInformation("Starting animation: {Name}, Color: {Color}, Duration: {Duration}s", 
                request.Name, colorString, request.Duration);
        }
        
        _controller.StartAnimation(request.Name, color, request.Duration);
        
        return Ok(new
        {
            status = "ok",
            sequence = request.Name,
            color = colorDescription,
            duration = request.Duration
        });
    }
}
