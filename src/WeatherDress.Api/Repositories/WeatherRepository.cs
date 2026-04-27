using System.Text.Json;
using WeatherDress.Api.Models;

namespace WeatherDress.Api.Repositories;

public class WeatherRepository : IWeatherRepository
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public WeatherRepository(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenWeather:ApiKey"];
    }

    public WeatherForecast GetForecastByZip(string zipCode)
    {
        var url = $"https://api.openweathermap.org/data/2.5/weather?zip={zipCode},dk&appid={_apiKey}&units=metric&lang=da";
        var response = _httpClient.GetAsync(url).Result;

        if (!response.IsSuccessStatusCode)
            throw new ArgumentException($"Postnummer '{zipCode}' blev ikke fundet i Danmark.");

        var json = response.Content.ReadAsStringAsync().Result;
        var root = JsonDocument.Parse(json).RootElement;

        return new WeatherForecast
        {
            Location = root.GetProperty("name").GetString(),
            TemperatureC = root.GetProperty("main").GetProperty("temp").GetDouble(),
            Description = root.GetProperty("weather")[0].GetProperty("description").GetString(),
            Icon = root.GetProperty("weather")[0].GetProperty("icon").GetString(),
            WindSpeed = root.GetProperty("wind").GetProperty("speed").GetDouble(),
            Humidity = root.GetProperty("main").GetProperty("humidity").GetInt32()
        };
    }
}
