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

    [HttpGet("{zipCode}/today")]
    public IActionResult GetToday(string zipCode)
    {
        try
        {
            var forecasts = _weatherRepository.GetTodayForecast(zipCode);
            return Ok(forecasts);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{zipCode}/yesterday")]
    public IActionResult GetYesterday(string zipCode)
    {
        try
        {
            var forecasts = _weatherRepository.GetYesterdayForecast(zipCode);
            return Ok(forecasts);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{zipCode}/clothing-position")]
    public IActionResult GetClothingPosition(string zipCode)
    {
        try
        {
            var recommendation = _weatherRepository.GetClothingRecommendation(zipCode);
            return Ok(recommendation);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
