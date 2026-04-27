using WeatherDress.Api.Models;

namespace WeatherDress.Api.Repositories;

public interface IWeatherRepository
{
    WeatherForecast GetForecastByZip(string zipCode);
}
