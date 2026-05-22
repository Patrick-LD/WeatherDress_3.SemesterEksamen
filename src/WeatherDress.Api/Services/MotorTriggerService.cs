namespace WeatherDress.Api.Services;

public class MotorTriggerService : IMotorTriggerService
{
    private volatile bool _triggered = false;
    private string? _jacket;
    private string? _pants;
    private string? _shoes;

    public void Trigger(string jacket, string pants, string shoes)
    {
        _jacket = jacket;
        _pants = pants;
        _shoes = shoes;
        _triggered = true;
    }

    public (bool Triggered, string? Jacket, string? Pants, string? Shoes) ConsumeIfTriggered()
    {
        if (!_triggered) return (false, null, null, null);
        _triggered = false;
        return (true, _jacket, _pants, _shoes);
    }
}
