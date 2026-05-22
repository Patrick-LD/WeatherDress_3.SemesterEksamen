namespace WeatherDress.Api.Services;

public interface IMotorTriggerService
{
    void Trigger(string jacket, string pants, string shoes);
    (bool Triggered, string? Jacket, string? Pants, string? Shoes) ConsumeIfTriggered();
}
