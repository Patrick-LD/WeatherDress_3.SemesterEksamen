using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherDress.Api.Controllers;
using WeatherDress.Api.Models;
using WeatherDress.Api.Repositories;
using WeatherDress.Api.Services;

namespace WeatherDress.UnitTests.Controllers;

public class MotorControllerTests
{
    private readonly Mock<IMotorTriggerService> _motorService = new();
    private readonly Mock<IWeatherRepository> _weatherRepo = new();

    private MotorController CreateController() => new(_motorService.Object);

    [Fact]
    public void PostTrigger_CallsTriggerServiceOgReturner200()
    {
        var rec = new ClothingRecommendation { Jacket = "T-shirt", Pants = "Shorts", Shoes = "Sandaler" };
        _weatherRepo.Setup(r => r.GetClothingRecommendation("1234")).Returns(rec);

        var result = CreateController().PostTrigger(new TriggerRequest("1234"), _weatherRepo.Object);

        _motorService.Verify(s => s.Trigger("T-shirt", "Shorts", "Sandaler"), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetStatus_NaarIkkeTriggered_ReturnsFalse()
    {
        _motorService.Setup(s => s.ConsumeIfTriggered()).Returns((false, null, null, null));

        var result = CreateController().GetStatus();

        var ok = Assert.IsType<OkObjectResult>(result);
        var triggered = ok.Value!.GetType().GetProperty("triggered")!.GetValue(ok.Value);
        Assert.Equal(false, triggered);
    }

    [Fact]
    public void GetStatus_NaarTriggered_ReturnsTrue()
    {
        _motorService.Setup(s => s.ConsumeIfTriggered()).Returns((true, "T-shirt", "Shorts", "Sandaler"));

        var result = CreateController().GetStatus();

        var ok = Assert.IsType<OkObjectResult>(result);
        var triggered = ok.Value!.GetType().GetProperty("triggered")!.GetValue(ok.Value);
        Assert.Equal(true, triggered);
    }
}
