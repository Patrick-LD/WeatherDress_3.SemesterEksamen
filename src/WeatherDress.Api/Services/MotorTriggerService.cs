namespace WeatherDress.Api.Services;

public class MotorTriggerService : IMotorTriggerService
{
    private volatile bool _triggered = false;

    public void Trigger() => _triggered = true;

    public bool ConsumeIfTriggered()
    {
        if (!_triggered) return false;
        _triggered = false;
        return true;
    }
}
