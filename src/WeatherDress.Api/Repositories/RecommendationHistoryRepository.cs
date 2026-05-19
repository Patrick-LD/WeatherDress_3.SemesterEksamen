using Microsoft.EntityFrameworkCore;
using WeatherDress.Api.Data;
using WeatherDress.Api.Models;

namespace WeatherDress.Api.Repositories;

public class RecommendationHistoryRepository : IRecommendationHistoryRepository
{
    private readonly WeatherDressDbContext _db;

    public RecommendationHistoryRepository(WeatherDressDbContext db)
    {
        _db = db;
    }

    public async Task UpsertAsync(DailyRecommendation recommendation)
    {
        var existing = await _db.DailyRecommendations
            .FirstOrDefaultAsync(r => r.ZipCode == recommendation.ZipCode && r.Date == recommendation.Date);

        if (existing is null)
        {
            _db.DailyRecommendations.Add(recommendation);
        }
        else
        {
            existing.Location = recommendation.Location;
            existing.TemperatureC = recommendation.TemperatureC;
            existing.WeatherDescription = recommendation.WeatherDescription;
            existing.WeatherCategory = recommendation.WeatherCategory;
            existing.Jacket = recommendation.Jacket;
            existing.Pants = recommendation.Pants;
            existing.Shoes = recommendation.Shoes;
            existing.ShoesNote = recommendation.ShoesNote;
            existing.SavedAt = recommendation.SavedAt;
        }

        await _db.SaveChangesAsync();
    }

    public async Task<List<DailyRecommendation>> GetLast7DaysAsync(string zipCode)
    {
        var cutoff = DateOnly.FromDateTime(DateTime.Today.AddDays(-6));
        return await _db.DailyRecommendations
            .Where(r => r.ZipCode == zipCode && r.Date >= cutoff)
            .OrderByDescending(r => r.Date)
            .ToListAsync();
    }

    public async Task<List<string>> GetActiveZipCodesAsync()
    {
        var cutoff = DateOnly.FromDateTime(DateTime.Today.AddDays(-7));
        return await _db.DailyRecommendations
            .Where(r => r.Date >= cutoff)
            .Select(r => r.ZipCode)
            .Distinct()
            .ToListAsync();
    }
}
