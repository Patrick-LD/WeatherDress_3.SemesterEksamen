using System.Net;
using Microsoft.Extensions.Configuration;
using WeatherDress.Api.Repositories;

namespace WeatherDress.UnitTests;

public class WeatherRepositoryTests
{
    [Fact]
    public void GetForecastByZip_ValidZip_ReturnsWeatherForecast()
    {
        // Arrange
        var fakeJson = "{\"name\":\"Copenhagen\",\"main\":{\"temp\":15.0,\"humidity\":80},\"weather\":[{\"description\":\"klar himmel\",\"icon\":\"01d\"}],\"wind\":{\"speed\":5.0}}";
        var repo = CreateRepo(fakeJson, HttpStatusCode.OK);

        // Act
        var result = repo.GetForecastByZip("2100");

        // Assert
        Assert.Equal("Copenhagen", result.Location);
        Assert.Equal(15.0, result.TemperatureC);
    }

    [Fact]
    public void GetForecastByZip_InvalidZip_ThrowsArgumentException()
    {
        // Arrange
        var repo = CreateRepo("", HttpStatusCode.NotFound);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => repo.GetForecastByZip("9999"));
    }

    private WeatherRepository CreateRepo(string fakeJson, HttpStatusCode statusCode)
    {
        var httpClient = new HttpClient(new FakeHttpHandler(fakeJson, statusCode));
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { { "OpenWeather:ApiKey", "testkey" } })
            .Build();
        return new WeatherRepository(httpClient, config);
    }
}

public class FakeHttpHandler : HttpMessageHandler
{
    private readonly string _json;
    private readonly HttpStatusCode _statusCode;

    public FakeHttpHandler(string json, HttpStatusCode statusCode)
    {
        _json = json;
        _statusCode = statusCode;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = _statusCode,
            Content = new StringContent(_json)
        });
    }
}
