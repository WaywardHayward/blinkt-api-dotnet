using System.Drawing;
using Microsoft.AspNetCore.Mvc;
using BlinktApi.Models;
using BlinktApi.Services;

namespace BlinktApi.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class QueueController : ControllerBase
{
    private readonly AnimationController _controller;
    private readonly ILogger<QueueController> _logger;

    public QueueController(AnimationController controller, ILogger<QueueController> logger)
    {
        _controller = controller;
        _logger = logger;
    }

    /// <summary>
    /// Add animations to the queue
    /// </summary>
    /// <param name="request">List of animations to queue</param>
    /// <returns>Queue confirmation</returns>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public IActionResult Post([FromBody] QueueRequest request)
    {
        var queuedAnimations = new List<QueuedAnimation>();
        
        foreach (var item in request.Queue)
        {
            // Parse color
            Color color;
            if (item.Rgb != null)
            {
                color = ColorHelper.FromRgb(item.Rgb.R, item.Rgb.G, item.Rgb.B);
            }
            else
            {
                var colorString = item.Color ?? "blue";
                color = ColorHelper.Parse(colorString);
            }
            
            queuedAnimations.Add(new QueuedAnimation
            {
                Name = item.Name,
                Color = color,
                DurationSeconds = item.Duration
            });
        }
        
        _controller.EnqueueAnimations(queuedAnimations);
        
        _logger.LogInformation("Queued {Count} animations", queuedAnimations.Count);
        
        return Ok(new
        {
            status = "ok",
            queued = queuedAnimations.Count,
            total_queue_length = _controller.GetQueueLength()
        });
    }

    /// <summary>
    /// Clear the animation queue
    /// </summary>
    [HttpDelete]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult Delete()
    {
        var previousLength = _controller.GetQueueLength();
        _controller.ClearQueue();
        
        _logger.LogInformation("Cleared queue ({Count} items)", previousLength);
        
        return Ok(new
        {
            status = "ok",
            cleared = previousLength
        });
    }
}
