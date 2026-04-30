# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

WeatherDress is a .NET 9 ASP.NET Core Web API that returns hourly weather data for Danish zip codes. It is a 3rd semester exam project.

## Commands

```bash
# Restore dependencies
dotnet restore WeatherDress.sln

# Build (Release)
dotnet build --configuration Release --no-restore

# Run the API (development)
dotnet run --project src/WeatherDress.Api

# Run unit tests
dotnet test tests/WeatherDress.UnitTests

# Run a single test
dotnet test tests/WeatherDress.UnitTests --filter "FullyQualifiedName~GetTodayForecast_ValidZip_ReturnsList"

# Run UI/Selenium tests (requires Chrome locally — excluded from CI)
dotnet test tests/WeatherDress.UITests
```

API is served at `https://localhost:7144` (HTTPS) or `http://localhost:5058` (HTTP). Swagger UI is available at `/swagger` in development.

## Architecture

The project follows the **Repository Pattern** with constructor-injected `HttpClient`:

```
Controller  →  IWeatherRepository  →  External APIs
```

- **`WeatherForecastController`** — Two endpoints: `GET /api/weatherforecast/{zipCode}/today` and `GET /api/weatherforecast/{zipCode}/yesterday`. Catches `ArgumentException` (invalid zip) and re-throws other exceptions.
- **`WeatherRepository`** — Implements `IWeatherRepository`. Calls two external APIs sequentially: first Dataforsyningen to resolve a Danish zip code to lat/lon, then Open-Meteo for hourly forecast or archive data. Maps WMO weather codes to Danish descriptions.
- **`WeatherForecast`** — Flat DTO: `Location`, `Date`, `Time`, `TemperatureC`, `Description`, `WindSpeed`, `Humidity`, `Precipitation`.

`WeatherRepository` is registered as **Scoped** in `Program.cs`.

## External APIs (no auth required)

| API | Purpose |
|-----|---------|
| `api.dataforsyningen.dk/postnumre/{zipCode}` | Resolves Danish postal code → lat/lon + city name |
| `api.open-meteo.com/v1/forecast` | Current-day hourly weather |
| `archive-api.open-meteo.com/v1/archive` | Historical (yesterday) hourly weather |

An `ArgumentException` is thrown when the Dataforsyningen API returns no results for a zip code.

## Testing Strategy

**Unit tests** (`WeatherDress.UnitTests`, xUnit + Moq + FluentAssertions): Use a custom `FakeHttpHandler : HttpMessageHandler` to mock HTTP responses from both external APIs. Tests create a real `WeatherRepository` with the fake handler injected — no Moq mocking of the repository itself.

**UI/integration tests** (`WeatherDress.UITests`, NUnit + Selenium): Placeholder structure exists; not yet implemented. These are excluded from CI because they require Chrome.

## Branching & Commit Conventions

- **`main`** — production; protected, requires passing CI
- **`develop`** — active development; protected, requires passing CI
- Feature branches: `feature/<name>`, bugfix: `bugfix/<name>`, hotfix: `hotfix/<name>`
- Commit prefixes: `feat:`, `fix:`, `test:`, `docs:`, `refactor:`, `chore:`
- All merges go through PRs; no direct commits to `main` or `develop`

## CI Pipeline

`.github/workflows/ci-pipeline.yml` runs on push/PR to `main` or `develop`:
1. `dotnet restore`
2. `dotnet build --configuration Release`
3. `dotnet test tests/WeatherDress.UnitTests` (UI tests excluded)
