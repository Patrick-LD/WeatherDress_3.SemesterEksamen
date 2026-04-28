namespace WeatherDress.Api.Models;

public class WeatherForecast
{
    public string? Location { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    public double TemperatureC { get; set; }
    public string? Description { get; set; }
    public double WindSpeed { get; set; }
    public int Humidity { get; set; }
    public double Precipitation { get; set; }
}
