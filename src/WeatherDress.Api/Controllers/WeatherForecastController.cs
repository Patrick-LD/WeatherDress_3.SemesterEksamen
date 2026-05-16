using Microsoft.AspNetCore.Mvc;
using WeatherDress.Api.Models;
using WeatherDress.Api.Repositories;

namespace WeatherDress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherRepository _weatherRepository;
    private readonly IRecommendationHistoryRepository _historyRepository;

    public WeatherForecastController(IWeatherRepository weatherRepository, IRecommendationHistoryRepository historyRepository)
    {
        _weatherRepository = weatherRepository;
        _historyRepository = historyRepository;
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
    public async Task<IActionResult> GetClothingPosition(string zipCode)
    {
        try
        {
            var recommendation = _weatherRepository.GetClothingRecommendation(zipCode);
            await _historyRepository.UpsertAsync(new DailyRecommendation
            {
                ZipCode = zipCode,
                Location = recommendation.Location ?? string.Empty,
                Date = DateOnly.FromDateTime(DateTime.Today),
                TemperatureC = recommendation.CurrentTemperatureC,
                WeatherDescription = recommendation.CurrentDescription,
                WeatherCategory = recommendation.WeatherCategory,
                Jacket = recommendation.Jacket,
                Pants = recommendation.Pants,
                Shoes = recommendation.Shoes,
                ShoesNote = recommendation.ShoesNote,
                SavedAt = DateTime.UtcNow
            });
            return Ok(recommendation);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{zipCode}/recommendation-history")]
    public async Task<IActionResult> GetRecommendationHistory(string zipCode)
    {
        try
        {
            var history = await _historyRepository.GetLast7DaysAsync(zipCode);
            return Ok(history);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
