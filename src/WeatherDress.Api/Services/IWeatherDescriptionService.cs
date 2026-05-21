namespace WeatherDress.Api.Services;

public interface IWeatherDescriptionService
{
    string GetDescription(int weatherCode);
}
