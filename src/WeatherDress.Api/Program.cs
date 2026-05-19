using Microsoft.EntityFrameworkCore;
using WeatherDress.Api.Data;
using WeatherDress.Api.Repositories;
using WeatherDress.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IWeatherDescriptionService, WeatherDescriptionService>();
builder.Services.AddSingleton<IMotorTriggerService, MotorTriggerService>();
builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();
builder.Services.AddDbContext<WeatherDressDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IRecommendationHistoryRepository, RecommendationHistoryRepository>();
builder.Services.AddHostedService<DailyRecommendationBackgroundService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<WeatherDressDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database migration failed on startup");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();





app.Run();

