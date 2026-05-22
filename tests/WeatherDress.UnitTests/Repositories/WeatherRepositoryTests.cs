using System.Net;
using Microsoft.Extensions.Configuration;
using WeatherDress.Api.Repositories;
using WeatherDress.Api.Services;

namespace WeatherDress.UnitTests;

public class WeatherRepositoryTests
{
    private readonly string _coordinatesFakeJson = "{\"nr\":\"2100\",\"navn\":\"Copenhagen\",\"visueltcenter\":[12.5635,55.6786]}";
    private readonly string _todayFakeJson = "{\"hourly\":{\"time\":[\"2026-04-28T09:00\"],\"temperature_2m\":[15.0],\"weathercode\":[2],\"windspeed_10m\":[5.0],\"relativehumidity_2m\":[80],\"precipitation\":[0.0]}}";

    // Genererer 24-element forecast JSON så [DateTime.Now.Hour]-indeksering altid virker
    private static string BuildClothingWeatherJson(double temp, int code, double precip)
    {
        var temps = string.Join(",", Enumerable.Repeat(temp.ToString(System.Globalization.CultureInfo.InvariantCulture), 24));
        var codes = string.Join(",", Enumerable.Repeat(code, 24));
        var precips = string.Join(",", Enumerable.Repeat(precip.ToString(System.Globalization.CultureInfo.InvariantCulture), 24));
        return $"{{\"hourly\":{{\"temperature_2m\":[{temps}],\"weathercode\":[{codes}],\"precipitation\":[{precips}]}}}}";
    }

    private static string BuildRainYesterdayJson(double precip)
    {
        var precips = string.Join(",", Enumerable.Repeat(precip.ToString(System.Globalization.CultureInfo.InvariantCulture), 24));
        return $"{{\"hourly\":{{\"precipitation\":[{precips}]}}}}";
    }

    // --- GetTodayForecast ---

    [Fact]
    public void GetTodayForecast_ValidZip_ReturnsList()
    {
        var repo = CreateRepo(_coordinatesFakeJson, _todayFakeJson);

        var result = repo.GetTodayForecast("2100");

        Assert.NotEmpty(result);
        Assert.Equal(15.0, result[0].TemperatureC);
        Assert.Equal("delvist skyet", result[0].Description);
    }

    [Fact]
    public void GetTodayForecast_InvalidZip_ThrowsArgumentException()
    {
        var responses = new Queue<string>(new[] { "", _todayFakeJson });
        var handler = new FakeHttpHandler(responses, firstCallStatusCode: HttpStatusCode.NotFound);
        var repo = new WeatherRepository(new HttpClient(handler), new WeatherDescriptionService());

        Assert.Throws<ArgumentException>(() => repo.GetTodayForecast("9999"));
    }

    // --- GetYesterdayForecast ---

    [Fact]
    public void GetYesterdayForecast_ValidZip_ReturnsList()
    {
        var repo = CreateRepo(_coordinatesFakeJson, _todayFakeJson);

        var result = repo.GetYesterdayForecast("2100");

        Assert.NotEmpty(result);
        Assert.Equal(15.0, result[0].TemperatureC);
        Assert.Equal("delvist skyet", result[0].Description);
    }

    [Fact]
    public void GetYesterdayForecast_InvalidZip_ThrowsArgumentException()
    {
        var responses = new Queue<string>(new[] { "", _todayFakeJson });
        var handler = new FakeHttpHandler(responses, firstCallStatusCode: HttpStatusCode.NotFound);
        var repo = new WeatherRepository(new HttpClient(handler), new WeatherDescriptionService());

        Assert.Throws<ArgumentException>(() => repo.GetYesterdayForecast("9999"));
    }

    // --- GetClothingRecommendation ---

    [Fact]
    public void GetClothingRecommendation_ValidZip_HotWeather_ReturnsSummerClothes()
    {
        var responses = new Queue<string>(new[]
        {
            _coordinatesFakeJson,
            BuildClothingWeatherJson(temp: 25, code: 0, precip: 0),
            BuildRainYesterdayJson(precip: 0)
        });
        var repo = new WeatherRepository(new HttpClient(new FakeHttpHandler(responses)), new WeatherDescriptionService());

        var result = repo.GetClothingRecommendation("2100");

        Assert.Equal("T-shirt", result.Jacket);
        Assert.Equal("Shorts", result.Pants);
        Assert.Equal("Sandaler", result.Shoes);
        Assert.Equal("Sol/varmt", result.WeatherCategory);
        Assert.Null(result.ShoesNote);
    }

    [Fact]
    public void GetClothingRecommendation_ValidZip_RainyWeather_ReturnsRainClothes()
    {
        var responses = new Queue<string>(new[]
        {
            _coordinatesFakeJson,
            BuildClothingWeatherJson(temp: 15, code: 61, precip: 2.0),
            BuildRainYesterdayJson(precip: 0)
        });
        var repo = new WeatherRepository(new HttpClient(new FakeHttpHandler(responses)), new WeatherDescriptionService());

        var result = repo.GetClothingRecommendation("2100");

        Assert.Equal("Regnjakke", result.Jacket);
        Assert.Equal("Regnbukser", result.Pants);
        Assert.Equal("Gummistøvler", result.Shoes);
        Assert.Equal("Regn", result.WeatherCategory);
        Assert.Null(result.ShoesNote);
    }

    [Fact]
    public void GetClothingRecommendation_ValidZip_ColdWeather_ReturnsWinterClothes()
    {
        var responses = new Queue<string>(new[]
        {
            _coordinatesFakeJson,
            BuildClothingWeatherJson(temp: 2, code: 0, precip: 0),
            BuildRainYesterdayJson(precip: 0)
        });
        var repo = new WeatherRepository(new HttpClient(new FakeHttpHandler(responses)), new WeatherDescriptionService());

        var result = repo.GetClothingRecommendation("2100");

        Assert.Equal("Flyverdragt", result.Jacket);
        Assert.Equal("Regnbukser", result.Pants);
        Assert.Equal("Vinterstøvler", result.Shoes);
        Assert.Equal("Koldt/sne", result.WeatherCategory);
    }

    [Fact]
    public void GetClothingRecommendation_ValidZip_OvercastWeather_ReturnsDefaultClothes()
    {
        var responses = new Queue<string>(new[]
        {
            _coordinatesFakeJson,
            BuildClothingWeatherJson(temp: 14, code: 2, precip: 0),
            BuildRainYesterdayJson(precip: 0)
        });
        var repo = new WeatherRepository(new HttpClient(new FakeHttpHandler(responses)), new WeatherDescriptionService());

        var result = repo.GetClothingRecommendation("2100");

        Assert.Equal("Sweatshirt", result.Jacket);
        Assert.Equal("Jeans", result.Pants);
        Assert.Equal("Sneakers", result.Shoes);
        Assert.Equal("Overskyet", result.WeatherCategory);
        Assert.Null(result.ShoesNote);
    }

    [Fact]
    public void GetClothingRecommendation_ValidZip_RainedYesterday_ShoesHaveNote()
    {
        var responses = new Queue<string>(new[]
        {
            _coordinatesFakeJson,
            BuildClothingWeatherJson(temp: 14, code: 0, precip: 0),
            BuildRainYesterdayJson(precip: 1.0)
        });
        var repo = new WeatherRepository(new HttpClient(new FakeHttpHandler(responses)), new WeatherDescriptionService());

        var result = repo.GetClothingRecommendation("2100");

        Assert.Equal("Gummistøvler", result.Shoes);
        Assert.NotNull(result.ShoesNote);
    }

    [Fact]
    public void GetClothingRecommendation_InvalidZip_ThrowsArgumentException()
    {
        var responses = new Queue<string>(new[] { "", BuildClothingWeatherJson(14, 0, 0) });
        var handler = new FakeHttpHandler(responses, firstCallStatusCode: HttpStatusCode.NotFound);
        var repo = new WeatherRepository(new HttpClient(handler), new WeatherDescriptionService());

        Assert.Throws<ArgumentException>(() => repo.GetClothingRecommendation("9999"));
    }

    // --- Hjælpemetoder ---

    private WeatherRepository CreateRepo(string coordinatesJson, string weatherJson)
    {
        var responses = new Queue<string>(new[] { coordinatesJson, weatherJson });
        var handler = new FakeHttpHandler(responses);
        return new WeatherRepository(new HttpClient(handler), new WeatherDescriptionService());
    }
}

public class FakeHttpHandler : HttpMessageHandler
{
    private readonly Queue<string> _responses;
    private readonly HttpStatusCode _firstCallStatusCode;
    private bool _firstCall = true;

    public FakeHttpHandler(Queue<string> responses, HttpStatusCode firstCallStatusCode = HttpStatusCode.OK)
    {
        _responses = responses;
        _firstCallStatusCode = firstCallStatusCode;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var json = _responses.Dequeue();
        var statusCode = _firstCall ? _firstCallStatusCode : HttpStatusCode.OK;
        _firstCall = false;

        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(json)
        });
    }
}
