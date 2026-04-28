using WeatherDress.Api.Models;

namespace WeatherDress.Api.Repositories;

public interface IWeatherRepository
{
    List<WeatherForecast> GetTodayForecast(string zipCode);
    List<WeatherForecast> GetYesterdayForecast(string zipCode);
}
