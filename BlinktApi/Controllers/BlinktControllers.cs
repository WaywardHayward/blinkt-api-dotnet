using Microsoft.AspNetCore.Mvc;
using BlinktApi.Models;
using BlinktApi.Services;

namespace BlinktApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SequenceController : ControllerBase
{
    private readonly AnimationController _controller;

    public SequenceController(AnimationController controller)
    {
        _controller = controller;
    }

    [HttpPost]
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
public class StatusController : ControllerBase
{
    private readonly AnimationController _controller;

    public StatusController(AnimationController controller)
    {
        _controller = controller;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_controller.GetStatus());
    }
}

[ApiController]
[Route("[controller]")]
public class AnimationsController : ControllerBase
{
    private readonly AnimationController _controller;

    public AnimationsController(AnimationController controller)
    {
        _controller = controller;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var animations = _controller.GetAnimationNames();
        return Ok(new { animations });
    }

    [HttpGet("{name}")]
    public IActionResult Get(string name)
    {
        var animation = _controller.GetAnimation(name);
        if (animation == null)
            return NotFound();
            
        return Ok(new { animation });
    }
}
