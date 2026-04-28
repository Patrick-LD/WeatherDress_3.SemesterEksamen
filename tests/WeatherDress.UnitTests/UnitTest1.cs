using System.Net;
using Microsoft.Extensions.Configuration;
using WeatherDress.Api.Repositories;

namespace WeatherDress.UnitTests;

public class WeatherRepositoryTests
{
    private readonly string _todayFakeJson = "{\"hourly\":{\"time\":[\"2026-04-28T09:00\"],\"temperature_2m\":[15.0],\"weathercode\":[2],\"windspeed_10m\":[5.0],\"relativehumidity_2m\":[80],\"precipitation\":[0.0]}}";
    private readonly string _coordinatesFakeJson = "[{\"lat\":\"55.67\",\"lon\":\"12.56\",\"display_name\":\"Copenhagen, Denmark\"}]";

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
        var repo = CreateRepo("[]", _todayFakeJson);

        Assert.Throws<ArgumentException>(() => repo.GetTodayForecast("9999"));
    }

    private WeatherRepository CreateRepo(string coordinatesJson, string weatherJson)
    {
        var responses = new Queue<string>(new[] { coordinatesJson, weatherJson });
        var handler = new FakeHttpHandler(responses);
        var httpClient = new HttpClient(handler);
        return new WeatherRepository(httpClient);
    }
}

public class FakeHttpHandler : HttpMessageHandler
{
    private readonly Queue<string> _responses;

    public FakeHttpHandler(Queue<string> responses)
    {
        _responses = responses;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var json = _responses.Dequeue();
        var statusCode = json == "[]" ? HttpStatusCode.OK : HttpStatusCode.OK;

        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(json)
        });
    }
}
