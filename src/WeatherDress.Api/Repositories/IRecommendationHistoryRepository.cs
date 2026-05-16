using WeatherDress.Api.Models;

namespace WeatherDress.Api.Repositories;

public interface IRecommendationHistoryRepository
{
    Task UpsertAsync(DailyRecommendation recommendation);
    Task<List<DailyRecommendation>> GetLast7DaysAsync(string zipCode);
    Task<List<string>> GetActiveZipCodesAsync();
}
