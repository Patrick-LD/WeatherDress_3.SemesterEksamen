using Microsoft.EntityFrameworkCore;
using WeatherDress.Api.Models;

namespace WeatherDress.Api.Data;

public class WeatherDressDbContext : DbContext
{
    public WeatherDressDbContext(DbContextOptions<WeatherDressDbContext> options) : base(options) { }

    public DbSet<DailyRecommendation> DailyRecommendations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DailyRecommendation>()
            .HasIndex(r => new { r.ZipCode, r.Date })
            .IsUnique();
    }
}
