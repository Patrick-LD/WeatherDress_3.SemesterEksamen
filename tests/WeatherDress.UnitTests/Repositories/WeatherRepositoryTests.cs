using System.Net;
using Microsoft.Extensions.Configuration;
using WeatherDress.Api.Repositories;
using WeatherDress.Api.Services;

namespace WeatherDress.UnitTests;

public class WeatherRepositoryTests
{
    private readonly string _todayFakeJson = "{\"hourly\":{\"time\":[\"2026-04-28T09:00\"],\"temperature_2m\":[15.0],\"weathercode\":[2],\"windspeed_10m\":[5.0],\"relativehumidity_2m\":[80],\"precipitation\":[0.0]}}";
    private readonly string _coordinatesFakeJson = "{\"nr\":\"2100\",\"navn\":\"Copenhagen\",\"visueltcenter\":[12.5635,55.6786]}";

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
        var responses = new Queue<string>(new[] { "" , _todayFakeJson });
        var handler = new FakeHttpHandler(responses, firstCallStatusCode: System.Net.HttpStatusCode.NotFound);
        var repo = new WeatherRepository(new HttpClient(handler), new WeatherDescriptionService());

        Assert.Throws<ArgumentException>(() => repo.GetTodayForecast("9999"));
    }

    private WeatherRepository CreateRepo(string coordinatesJson, string weatherJson)
    {
        var responses = new Queue<string>(new[] { coordinatesJson, weatherJson });
        var handler = new FakeHttpHandler(responses);
        var httpClient = new HttpClient(handler);
        return new WeatherRepository(httpClient, new WeatherDescriptionService());
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
