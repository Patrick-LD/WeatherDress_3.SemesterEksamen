using System.Text.Json;
using WeatherDress.Api.Models;
using WeatherDress.Api.Services;

namespace WeatherDress.Api.Repositories;

public class WeatherRepository : IWeatherRepository
{
    private readonly HttpClient _httpClient;
    private readonly IWeatherDescriptionService _descriptionService;

    public WeatherRepository(HttpClient httpClient, IWeatherDescriptionService descriptionService)
    {
        _httpClient = httpClient;
        _descriptionService = descriptionService;
    }

    // Konverterer postnummer til koordinater via Nominatim (OpenStreetMap)
    private (double lat, double lon, string city) GetCoordinates(string zipCode)
    {
        var url = $"https://api.dataforsyningen.dk/postnumre/{zipCode}";
        var response = _httpClient.GetAsync(url).Result;

        if (!response.IsSuccessStatusCode)
            throw new ArgumentException($"Postnummer '{zipCode}' blev ikke fundet i Danmark.");

        var json = response.Content.ReadAsStringAsync().Result;
        var root = JsonDocument.Parse(json).RootElement;

        var center = root.GetProperty("visueltcenter");
        var lon = Math.Round(center[0].GetDouble(), 4);
        var lat = Math.Round(center[1].GetDouble(), 4);
        var city = root.GetProperty("navn").GetString()!;

        return (lat, lon, city);
    }

    public List<WeatherForecast> GetTodayForecast(string zipCode)
    {
        var (lat, lon, city) = GetCoordinates(zipCode);
        var today = DateTime.Now.ToString("yyyy-MM-dd");

        var url = $"https://api.open-meteo.com/v1/forecast?latitude={lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}&hourly=temperature_2m,weathercode,windspeed_10m,relativehumidity_2m,precipitation&timezone=Europe%2FCopenhagen&forecast_days=1";
        var response = _httpClient.GetAsync(url).Result;
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Vejr API fejlede: {response.StatusCode}");
        var json = response.Content.ReadAsStringAsync().Result;
        var hourly = JsonDocument.Parse(json).RootElement.GetProperty("hourly");

        var times = hourly.GetProperty("time").EnumerateArray().ToList();
        var temps = hourly.GetProperty("temperature_2m").EnumerateArray().ToList();
        var codes = hourly.GetProperty("weathercode").EnumerateArray().ToList();
        var winds = hourly.GetProperty("windspeed_10m").EnumerateArray().ToList();
        var humidity = hourly.GetProperty("relativehumidity_2m").EnumerateArray().ToList();
        var precip = hourly.GetProperty("precipitation").EnumerateArray().ToList();

        var forecasts = new List<WeatherForecast>();

        for (int i = 0; i < times.Count; i++)
        {
            forecasts.Add(new WeatherForecast
            {
                Location = city,
                Date = today,
                Time = times[i].GetString()!.Substring(11, 5),
                TemperatureC = temps[i].GetDouble(),
                Description = _descriptionService.GetDescription(codes[i].GetInt32()),
                WindSpeed = winds[i].GetDouble(),
                Humidity = humidity[i].GetInt32(),
                Precipitation = precip[i].GetDouble()
            });
        }

        return forecasts;
    }

    public List<WeatherForecast> GetYesterdayForecast(string zipCode)
    {
        var (lat, lon, city) = GetCoordinates(zipCode);
        var yesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

        var url = $"https://archive-api.open-meteo.com/v1/archive?latitude={lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}&start_date={yesterday}&end_date={yesterday}&hourly=temperature_2m,weathercode,windspeed_10m,relativehumidity_2m,precipitation&timezone=Europe%2FCopenhagen";
        var response = _httpClient.GetAsync(url).Result;
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Arkiv API fejlede: {response.StatusCode}");
        var json = response.Content.ReadAsStringAsync().Result;
        var hourly = JsonDocument.Parse(json).RootElement.GetProperty("hourly");

        var times = hourly.GetProperty("time").EnumerateArray().ToList();
        var temps = hourly.GetProperty("temperature_2m").EnumerateArray().ToList();
        var codes = hourly.GetProperty("weathercode").EnumerateArray().ToList();
        var winds = hourly.GetProperty("windspeed_10m").EnumerateArray().ToList();
        var humidity = hourly.GetProperty("relativehumidity_2m").EnumerateArray().ToList();
        var precip = hourly.GetProperty("precipitation").EnumerateArray().ToList();

        var forecasts = new List<WeatherForecast>();

        for (int i = 0; i < times.Count; i++)
        {
            forecasts.Add(new WeatherForecast
            {
                Location = city,
                Date = yesterday,
                Time = times[i].GetString()!.Substring(11, 5),
                TemperatureC = temps[i].GetDouble(),
                Description = _descriptionService.GetDescription(codes[i].GetInt32()),
                WindSpeed = winds[i].GetDouble(),
                Humidity = humidity[i].GetInt32(),
                Precipitation = precip[i].GetDouble()
            });
        }

        return forecasts;
    }
}
