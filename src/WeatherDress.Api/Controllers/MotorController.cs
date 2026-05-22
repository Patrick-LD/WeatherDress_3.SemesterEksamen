using Microsoft.AspNetCore.Mvc;
using WeatherDress.Api.Repositories;
using WeatherDress.Api.Services;

namespace WeatherDress.Api.Controllers;

public record TriggerRequest(string ZipCode);

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
    public IActionResult PostTrigger([FromBody] TriggerRequest req, [FromServices] IWeatherRepository weatherRepo)
    {
        var rec = weatherRepo.GetClothingRecommendation(req.ZipCode);
        _motorTriggerService.Trigger(rec.Jacket!, rec.Pants!, rec.Shoes!);
        return Ok(new { message = "Motor trigger sendt." });
    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var (triggered, jacket, pants, shoes) = _motorTriggerService.ConsumeIfTriggered();
        return Ok(new { triggered, jacket, pants, shoes });
    }
}
