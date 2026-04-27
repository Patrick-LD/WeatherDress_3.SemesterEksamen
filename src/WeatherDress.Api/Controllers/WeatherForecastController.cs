using Microsoft.AspNetCore.Mvc;
using WeatherDress.Api.Repositories;

namespace WeatherDress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherRepository _weatherRepository;

    public WeatherForecastController(IWeatherRepository weatherRepository)
    {
        _weatherRepository = weatherRepository;
    }

    [HttpGet("{zipCode}")]
    public IActionResult GetForecast(string zipCode)
    {
        try
        {
            var forecast = _weatherRepository.GetForecastByZip(zipCode);
            return Ok(forecast);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
