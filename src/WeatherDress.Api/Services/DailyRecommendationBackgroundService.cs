using WeatherDress.Api.Models;
using WeatherDress.Api.Repositories;

namespace WeatherDress.Api.Services;

public class DailyRecommendationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DailyRecommendationBackgroundService> _logger;

    public DailyRecommendationBackgroundService(IServiceScopeFactory scopeFactory, ILogger<DailyRecommendationBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = TimeUntilMidnight();
            await Task.Delay(delay, stoppingToken);

            if (!stoppingToken.IsCancellationRequested)
                await RefreshAllZipCodesAsync(stoppingToken);
        }
    }

    private async Task RefreshAllZipCodesAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var historyRepo = scope.ServiceProvider.GetRequiredService<IRecommendationHistoryRepository>();
        var weatherRepo = scope.ServiceProvider.GetRequiredService<IWeatherRepository>();

        var zipCodes = await historyRepo.GetActiveZipCodesAsync();
        _logger.LogInformation("Daily refresh: processing {Count} zip codes", zipCodes.Count);

        foreach (var zip in zipCodes)
        {
            if (ct.IsCancellationRequested) break;
            try
            {
                var clothing = weatherRepo.GetClothingRecommendation(zip);
                await historyRepo.UpsertAsync(MapToDaily(zip, clothing));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to refresh recommendation for zip {Zip}", zip);
            }
        }
    }

    private static DailyRecommendation MapToDaily(string zipCode, ClothingRecommendation c) => new()
    {
        ZipCode = zipCode,
        Location = c.Location ?? string.Empty,
        Date = DateOnly.FromDateTime(DateTime.Today),
        TemperatureC = c.CurrentTemperatureC,
        WeatherDescription = c.CurrentDescription,
        WeatherCategory = c.WeatherCategory,
        Jacket = c.Jacket,
        Pants = c.Pants,
        Shoes = c.Shoes,
        ShoesNote = c.ShoesNote,
        SavedAt = DateTime.UtcNow
    };

    private static TimeSpan TimeUntilMidnight()
    {
        var now = DateTime.Now;
        var midnight = now.Date.AddDays(1);
        return midnight - now;
    }
}
