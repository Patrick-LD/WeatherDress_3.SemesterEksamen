using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherDress.Api.Controllers;
using WeatherDress.Api.Models;
using WeatherDress.Api.Repositories;

namespace WeatherDress.UnitTests.Controllers;

public class WeatherForecastControllerTests
{
    private readonly Mock<IWeatherRepository> _weatherRepo = new();
    private readonly Mock<IRecommendationHistoryRepository> _historyRepo = new();

    private WeatherForecastController CreateController() =>
        new(_weatherRepo.Object, _historyRepo.Object);

    // --- GetToday ---

    [Fact]
    public void GetToday_ValidZip_Returns200()
    {
        _weatherRepo.Setup(r => r.GetTodayForecast("2100"))
            .Returns(new List<WeatherForecast> { new() { TemperatureC = 15 } });

        var result = CreateController().GetToday("2100");

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetToday_InvalidZip_Returns400()
    {
        _weatherRepo.Setup(r => r.GetTodayForecast("9999"))
            .Throws(new ArgumentException("Ugyldigt postnummer"));

        var result = CreateController().GetToday("9999");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- GetYesterday ---

    [Fact]
    public void GetYesterday_ValidZip_Returns200()
    {
        _weatherRepo.Setup(r => r.GetYesterdayForecast("2100"))
            .Returns(new List<WeatherForecast> { new() { TemperatureC = 12 } });

        var result = CreateController().GetYesterday("2100");

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetYesterday_InvalidZip_Returns400()
    {
        _weatherRepo.Setup(r => r.GetYesterdayForecast("9999"))
            .Throws(new ArgumentException("Ugyldigt postnummer"));

        var result = CreateController().GetYesterday("9999");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- GetClothingPosition ---

    [Fact]
    public async Task GetClothingPosition_ValidZip_Returns200()
    {
        _weatherRepo.Setup(r => r.GetClothingRecommendation("2100"))
            .Returns(new ClothingRecommendation { Location = "Copenhagen" });
        _historyRepo.Setup(r => r.UpsertAsync(It.IsAny<DailyRecommendation>()))
            .Returns(Task.CompletedTask);

        var result = await CreateController().GetClothingPosition("2100");

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetClothingPosition_ValidZip_CallsUpsertOnce()
    {
        _weatherRepo.Setup(r => r.GetClothingRecommendation("2100"))
            .Returns(new ClothingRecommendation { Location = "Copenhagen" });
        _historyRepo.Setup(r => r.UpsertAsync(It.IsAny<DailyRecommendation>()))
            .Returns(Task.CompletedTask);

        await CreateController().GetClothingPosition("2100");

        _historyRepo.Verify(
            r => r.UpsertAsync(It.Is<DailyRecommendation>(d => d.ZipCode == "2100")),
            Times.Once);
    }

    [Fact]
    public async Task GetClothingPosition_InvalidZip_Returns400()
    {
        _weatherRepo.Setup(r => r.GetClothingRecommendation("9999"))
            .Throws(new ArgumentException("Ugyldigt postnummer"));

        var result = await CreateController().GetClothingPosition("9999");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- GetRecommendationHistory ---

    [Fact]
    public async Task GetRecommendationHistory_ValidZip_Returns200()
    {
        _historyRepo.Setup(r => r.GetLast7DaysAsync("2100"))
            .ReturnsAsync(new List<DailyRecommendation>
            {
                new() { ZipCode = "2100", Location = "Copenhagen" },
                new() { ZipCode = "2100", Location = "Copenhagen" }
            });

        var result = await CreateController().GetRecommendationHistory("2100");

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetRecommendationHistory_InvalidZip_Returns400()
    {
        _historyRepo.Setup(r => r.GetLast7DaysAsync("9999"))
            .ThrowsAsync(new ArgumentException("Ugyldigt postnummer"));

        var result = await CreateController().GetRecommendationHistory("9999");

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
