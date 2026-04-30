namespace WeatherDress.Api.Services;

public class WeatherDescriptionService : IWeatherDescriptionService
{
    public string GetDescription(int code)
    {
        return code switch
        {
            0 => "klar himmel",
            1 => "overvejende klar",
            2 => "delvist skyet",
            3 => "overskyet",
            45 or 48 => "tåge",
            51 or 53 or 55 => "let støvregn",
            61 or 63 or 65 => "regn",
            71 or 73 or 75 => "sne",
            80 or 81 or 82 => "regnbyger",
            95 => "tordenvejr",
            _ => "ukendt"
        };
    }
}
