using System.Text.Json;
using WeatherDress.Api.Models;
using WeatherDress.Api.Services;
using System.Globalization;

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

        var url = $"https://api.open-meteo.com/v1/forecast?latitude={lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}&hourly=temperature_2m,weathercode,windspeed_10m,relativehumidity_2m,precipitation&timezone=Europe%2FCopenhagen&forecast_days=2";
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
            var timestamp = times[i].GetString()!;
            forecasts.Add(new WeatherForecast
            {
                Location = city,
                Date = timestamp.Substring(0, 10),
                Time = timestamp.Substring(11, 5),
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

    public ClothingRecommendation GetClothingRecommendation(string zipCode)
    {
        var (lat, lon, city) = GetCoordinates(zipCode);
        var currentHour = DateTime.Now.Hour;

        // Hent dagens vejrdata
        var url = $"https://api.open-meteo.com/v1/forecast?latitude={lat.ToString(CultureInfo.InvariantCulture)}&longitude={lon.ToString(CultureInfo.InvariantCulture)}&hourly=temperature_2m,weathercode,precipitation&timezone=Europe%2FCopenhagen&forecast_days=1";
        var response = _httpClient.GetAsync(url).Result;
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Vejr API fejlede: {response.StatusCode}");

        var json = response.Content.ReadAsStringAsync().Result;
        var hourly = JsonDocument.Parse(json).RootElement.GetProperty("hourly");

        var temp = hourly.GetProperty("temperature_2m").EnumerateArray().ToList()[currentHour].GetDouble();
        var code = hourly.GetProperty("weathercode").EnumerateArray().ToList()[currentHour].GetInt32();
        var rain = hourly.GetProperty("precipitation").EnumerateArray().ToList()[currentHour].GetDouble();
        var description = _descriptionService.GetDescription(code);

        // Tjek om det regnede i går (sko-hjulet bruger begge dages vejr)
        bool rainedYesterday = CheckedYesterdayForRain(lat, lon);

        // Jakke og bukser følger ALTID kun dagens vejr
        var (clothingPos, clothingAngle, category, jacket, pants) = DetermineJacketAndPants(temp, code, rain);

        // Sko følger: regn i dag ELLER regn i går = gummistøvler
        var (shoesPos, shoesAngle, shoes, shoesNote) = DetermineShoes(temp, code, rain, rainedYesterday);

        return new ClothingRecommendation
        {
            Location = city,
            JacketPosition = clothingPos,
            JacketMotorAngle = clothingAngle,
            Jacket = jacket,
            PantsPosition = clothingPos,
            PantsMotorAngle = clothingAngle,
            Pants = pants,
            ShoesPosition = shoesPos,
            ShoesMotorAngle = shoesAngle,
            Shoes = shoes,
            ShoesNote = shoesNote,
            WeatherCategory = category,
            CurrentTemperatureC = temp,
            CurrentDescription = description
        };
    }

    private bool CheckedYesterdayForRain(double lat, double lon)
    {
        var yesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
        var url = $"https://archive-api.open-meteo.com/v1/archive?latitude={lat.ToString(CultureInfo.InvariantCulture)}&longitude={lon.ToString(CultureInfo.InvariantCulture)}&start_date={yesterday}&end_date={yesterday}&hourly=precipitation&timezone=Europe%2FCopenhagen";

        try
        {
            var response = _httpClient.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode) return false;

            var json = response.Content.ReadAsStringAsync().Result;
            var precipList = JsonDocument.Parse(json).RootElement
                .GetProperty("hourly")
                .GetProperty("precipitation")
                .EnumerateArray();

            return precipList.Any(p => p.GetDouble() >= 0.5);
        }
        catch
        {
            return false;
        }
    }

    // WMO vejr-koder for regn: 51-67, 80-82, 95-99
    // WMO vejr-koder for sne: 71-77, 85-86
    private static bool IsRainCode(int code, double precipitation) =>
        (code >= 51 && code <= 67) || (code >= 80 && code <= 82) || code >= 95 || precipitation >= 0.5;

    private static bool IsSnowCode(int code) =>
        (code >= 71 && code <= 77) || code == 85 || code == 86;

    private static (int position, int angle, string category, string jacket, string pants)
        DetermineJacketAndPants(double temp, int code, double precipitation)
    {
        if (IsSnowCode(code) || temp < 5)
            return (4, 270, "Koldt/sne", "Flyverdragt", "Regnbukser");

        if (IsRainCode(code, precipitation))
            return (3, 180, "Regn", "Regnjakke", "Regnbukser");

        if (temp >= 20)
            return (1, 0, "Sol/varmt", "T-shirt", "Shorts");

        return (2, 90, "Overskyet", "Sweatshirt", "Jeans");
    }

    private static (int position, int angle, string shoes, string? note)
        DetermineShoes(double temp, int code, double precipitation, bool rainedYesterday)
    {
        if (IsSnowCode(code) || temp < 5)
            return (4, 270, "Vinterstøvler", null);

        if (IsRainCode(code, precipitation))
            return (3, 180, "Gummistøvler", null);

        if (rainedYesterday)
            return (3, 180, "Gummistøvler", "Det kan være en god idé at tage gummistøvler med, eftersom det regnede i går og det stadig kan være vådt udenfor.");

        if (temp >= 20)
            return (1, 0, "Sandaler", null);

        return (2, 90, "Sneakers", null);
    }
}
