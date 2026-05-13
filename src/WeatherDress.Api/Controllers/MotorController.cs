using Microsoft.AspNetCore.Mvc;
using WeatherDress.Api.Services;

namespace WeatherDress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MotorController : ControllerBase
{
    private readonly IMotorTriggerService _motorTriggerService;

    public MotorController(IMotorTriggerService motorTriggerService)
    {
        _motorTriggerService = motorTriggerService;
    }

    [HttpPost("trigger")]
    public IActionResult PostTrigger()
    {
        _motorTriggerService.Trigger();
        return Ok(new { message = "Motor trigger sendt." });
    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new { triggered = _motorTriggerService.ConsumeIfTriggered() });
    }
}
