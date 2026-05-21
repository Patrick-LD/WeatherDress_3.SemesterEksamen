namespace WeatherDress.Api.Models;

public class DailyRecommendation
{
    public int Id { get; set; }
    public string ZipCode { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public double TemperatureC { get; set; }
    public string? WeatherDescription { get; set; }
    public string? WeatherCategory { get; set; }
    public string? Jacket { get; set; }
    public string? Pants { get; set; }
    public string? Shoes { get; set; }
    public string? ShoesNote { get; set; }
    public DateTime SavedAt { get; set; }
}
