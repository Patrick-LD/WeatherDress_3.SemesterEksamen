namespace WeatherDress.Api.Services;

public interface IMotorTriggerService
{
    void Trigger();
    bool ConsumeIfTriggered();
}
