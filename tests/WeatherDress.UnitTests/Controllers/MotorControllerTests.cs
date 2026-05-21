using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherDress.Api.Controllers;
using WeatherDress.Api.Services;

namespace WeatherDress.UnitTests.Controllers;

public class MotorControllerTests
{
    private readonly Mock<IMotorTriggerService> _motorService = new();

    private MotorController CreateController() => new(_motorService.Object);

    [Fact]
    public void PostTrigger_CallsTriggerServiceOgReturner200()
    {
        var result = CreateController().PostTrigger();

        _motorService.Verify(s => s.Trigger(), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetStatus_NaarIkkeTriggered_ReturnsFalse()
    {
        _motorService.Setup(s => s.ConsumeIfTriggered()).Returns(false);

        var result = CreateController().GetStatus();

        var ok = Assert.IsType<OkObjectResult>(result);
        var triggered = ok.Value!.GetType().GetProperty("triggered")!.GetValue(ok.Value);
        Assert.Equal(false, triggered);
    }

    [Fact]
    public void GetStatus_NaarTriggered_ReturnsTrue()
    {
        _motorService.Setup(s => s.ConsumeIfTriggered()).Returns(true);

        var result = CreateController().GetStatus();

        var ok = Assert.IsType<OkObjectResult>(result);
        var triggered = ok.Value!.GetType().GetProperty("triggered")!.GetValue(ok.Value);
        Assert.Equal(true, triggered);
    }
}
