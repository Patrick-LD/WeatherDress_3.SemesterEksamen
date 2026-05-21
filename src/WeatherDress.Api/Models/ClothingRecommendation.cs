namespace WeatherDress.Api.Models;

public class ClothingRecommendation
{
    public string? Location { get; set; }

    // Motor 1 — Jakke-hjul (venstre)
    public int JacketPosition { get; set; }
    public int JacketMotorAngle { get; set; }
    public string? Jacket { get; set; }

    // Motor 2 — Bukser-hjul (midten)
    public int PantsPosition { get; set; }
    public int PantsMotorAngle { get; set; }
    public string? Pants { get; set; }

    // Motor 3 — Sko-hjul (højre) — følger OGSÅ gårsdagens vejr
    public int ShoesPosition { get; set; }
    public int ShoesMotorAngle { get; set; }
    public string? Shoes { get; set; }
    public string? ShoesNote { get; set; }

    public double CurrentTemperatureC { get; set; }
    public string? WeatherCategory { get; set; }
    public string? CurrentDescription { get; set; }
}
