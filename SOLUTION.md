# WeatherDress — Løsningsbeskrivelse

## Introduktion

WeatherDress er en .NET 9 ASP.NET Core Web API, der omdanner et dansk postnummer til en konkret tøjanbefaling baseret på det aktuelle vejr. Systemet driver tre fysiske motorhjul (jakke, bukser, sko) via en Raspberry Pi og gemmer daglige anbefalinger i en database. Brugeren tilgår løsningen via et React-frontend, der kalder API'et.

---

## Systemarkitektur

Løsningen følger **Repository Pattern** med constructor injection:

```
Klient (React)
    │
    ▼
WeatherForecastController
    │
    ├──▶ IWeatherRepository ──▶ api.dataforsyningen.dk (postnummer → lat/lon)
    │                        ──▶ api.open-meteo.com    (vejrudsigt i dag)
    │                        ──▶ archive-api.open-meteo.com (vejr i går)
    │
    ├──▶ IRecommendationHistoryRepository ──▶ SQL Server (EF Core)
    │
    └── DailyRecommendationBackgroundService (baggrundsjob, kører ved midnat)
```

**Afhængigheder registreres i `Program.cs`:**
- `WeatherRepository` og `RecommendationHistoryRepository` som **Scoped** (én instans per HTTP-request)
- `WeatherDescriptionService` og `MotorTriggerService` som **Singleton**
- `DailyRecommendationBackgroundService` som **Hosted Service**

---

## API Endpoints

| Endpoint | Beskrivelse |
|----------|-------------|
| `GET /api/weatherforecast/{zipCode}/today` | Timevejr for i dag |
| `GET /api/weatherforecast/{zipCode}/yesterday` | Timevejr for i går (arkiv-API) |
| `GET /api/weatherforecast/{zipCode}/clothing-position` | Tøjanbefaling + motorpositioner (gemmes i DB) |
| `GET /api/weatherforecast/{zipCode}/recommendation-history` | Seneste 7 dages anbefalinger fra DB |

Ugyldige postnumre returnerer `400 Bad Request`.

---

## Ekstern API-integration

`WeatherRepository` kalder to eksterne API'er sekventielt: først Dataforsyningen for at omsætte postnummeret til koordinater, derefter Open-Meteo for vejrdata.

**Trin 1 — Postnummer til koordinater:**

```csharp
private (double lat, double lon, string city) GetCoordinates(string zipCode)
{
    var url = $"https://api.dataforsyningen.dk/postnumre/{zipCode}";
    var response = _httpClient.GetAsync(url).Result;

    if (!response.IsSuccessStatusCode)
        throw new ArgumentException($"Postnummer '{zipCode}' blev ikke fundet i Danmark.");

    var json = response.Content.ReadAsStringAsync().Result;
    var root = JsonDocument.Parse(json).RootElement;

    var center = root.GetProperty("visueltcenter");
    var lon = Math.Round(center[0].GetDouble(), 4);
    var lat = Math.Round(center[1].GetDouble(), 4);
    var city = root.GetProperty("navn").GetString()!;

    return (lat, lon, city);
}
```

**Trin 2 — Timevejr fra Open-Meteo:**

```csharp
public List<WeatherForecast> GetTodayForecast(string zipCode)
{
    var (lat, lon, city) = GetCoordinates(zipCode);

    var url = $"https://api.open-meteo.com/v1/forecast" +
              $"?latitude={lat.ToString(CultureInfo.InvariantCulture)}" +
              $"&longitude={lon.ToString(CultureInfo.InvariantCulture)}" +
              $"&hourly=temperature_2m,weathercode,windspeed_10m,relativehumidity_2m,precipitation" +
              $"&timezone=Europe%2FCopenhagen&forecast_days=2";

    var json = _httpClient.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
    var hourly = JsonDocument.Parse(json).RootElement.GetProperty("hourly");

    var times    = hourly.GetProperty("time").EnumerateArray().ToList();
    var temps    = hourly.GetProperty("temperature_2m").EnumerateArray().ToList();
    var codes    = hourly.GetProperty("weathercode").EnumerateArray().ToList();
    // ... (vindstyrke, luftfugtighed, nedbør hentes tilsvarende)

    return times.Select((t, i) => new WeatherForecast
    {
        Location    = city,
        Date        = t.GetString()!.Substring(0, 10),
        Time        = t.GetString()!.Substring(11, 5),
        TemperatureC = temps[i].GetDouble(),
        Description = _descriptionService.GetDescription(codes[i].GetInt32()),
        // ...
    }).ToList();
}
```

---

## Tøjanbefalingslogik

Anbefalingen bestemmes af tre faktorer: **temperatur**, **WMO vejrkode** (international standard) og **nedbør i går**. Jakke og bukser følger udelukkende dagens vejr; sko tager også hensyn til gårsdagens nedbør (vådtjansen er stadig til stede).

Hvert udfald mapper til en **motorposition (1–4)** og en **vinkel (0°/90°/180°/270°)**, som styrer det fysiske hjul:

```csharp
// WMO-koder: regn = 51-67, 80-82, 95-99 | sne = 71-77, 85-86
private static bool IsRainCode(int code, double precipitation) =>
    (code >= 51 && code <= 67) || (code >= 80 && code <= 82)
    || code >= 95 || precipitation >= 0.5;

private static bool IsSnowCode(int code) =>
    (code >= 71 && code <= 77) || code == 85 || code == 86;

private static (int position, int angle, string category, string jacket, string pants)
    DetermineJacketAndPants(double temp, int code, double precipitation)
{
    if (IsSnowCode(code) || temp < 5)
        return (4, 270, "Koldt/sne", "Vinterjakke", "Flyverdragt");

    if (IsRainCode(code, precipitation))
        return (3, 180, "Regn", "Regnjakke", "Flyverdragt");

    if (temp >= 20)
        return (1, 0, "Sol/varmt", "T-shirt", "Shorts");

    return (2, 90, "Overskyet", "Hoodie", "Jeans");
}

private static (int position, int angle, string shoes, string? note)
    DetermineShoes(double temp, int code, double precipitation, bool rainedYesterday)
{
    if (IsSnowCode(code) || temp < 5)  return (4, 270, "Vinterstøvler", null);
    if (IsRainCode(code, precipitation)) return (3, 180, "Gummistøvler", null);
    if (rainedYesterday)
        return (3, 180, "Gummistøvler",
            "Det kan være en god idé at tage gummistøvler med, " +
            "eftersom det regnede i går og det stadig kan være vådt udenfor.");
    if (temp >= 20) return (1, 0, "Sandaler", null);
    return (2, 90, "Sneakers", null);
}
```

| Position | Vinkel | Vejrkategori |
|----------|--------|--------------|
| 1 | 0° | Sol / varmt (≥ 20°C) |
| 2 | 90° | Overskyet (5–19°C) |
| 3 | 180° | Regn |
| 4 | 270° | Koldt / sne (< 5°C) |

---

## Database og historik

Anbefalinger gemmes i SQL Server via **Entity Framework Core 9**. Tabellen har et unikt indeks på `(ZipCode, Date)`, hvilket muliggør et **upsert-mønster**: eksisterende post opdateres, ellers indsættes en ny.

Hvert kald til `/clothing-position` gemmer dagens anbefaling automatisk:

```csharp
[HttpGet("{zipCode}/clothing-position")]
public async Task<IActionResult> GetClothingPosition(string zipCode)
{
    var recommendation = _weatherRepository.GetClothingRecommendation(zipCode);

    await _historyRepository.UpsertAsync(new DailyRecommendation
    {
        ZipCode     = zipCode,
        Date        = DateOnly.FromDateTime(DateTime.Today),
        Jacket      = recommendation.Jacket,
        Pants       = recommendation.Pants,
        Shoes       = recommendation.Shoes,
        SavedAt     = DateTime.UtcNow,
        // ...
    });

    return Ok(recommendation);
}
```

**`DailyRecommendationBackgroundService`** kører som hosted service og opdaterer alle aktive postnumre automatisk ved midnat, så databasen altid har et frisk snapshot — selv for postnumre, der ikke er blevet slået op den dag:

```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        await Task.Delay(TimeUntilMidnight(), stoppingToken);
        await RefreshAllZipCodesAsync(stoppingToken);
    }
}
```

---

## Teststrategi

Unit-testene bruger **xUnit** og **FluentAssertions**. I stedet for at mocke `HttpClient` direkte (som ikke er en interface) implementeres en `FakeHttpHandler`, der erstatter den underliggende HTTP-transport med foruddefinerede JSON-svar i en kø:

```csharp
public class FakeHttpHandler : HttpMessageHandler
{
    private readonly Queue<string> _responses;
    private readonly HttpStatusCode _firstCallStatusCode;
    private bool _firstCall = true;

    public FakeHttpHandler(Queue<string> responses,
        HttpStatusCode firstCallStatusCode = HttpStatusCode.OK)
    {
        _responses = responses;
        _firstCallStatusCode = firstCallStatusCode;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var json = _responses.Dequeue();
        var statusCode = _firstCall ? _firstCallStatusCode : HttpStatusCode.OK;
        _firstCall = false;

        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content    = new StringContent(json)
        });
    }
}
```

Et eksempel på en test, der verificerer, at et ugyldigt postnummer kaster en `ArgumentException`:

```csharp
[Fact]
public void GetTodayForecast_InvalidZip_ThrowsArgumentException()
{
    var responses = new Queue<string>(new[] { "", _todayFakeJson });
    var handler = new FakeHttpHandler(responses, firstCallStatusCode: HttpStatusCode.NotFound);
    var repo = new WeatherRepository(new HttpClient(handler), new WeatherDescriptionService());

    Assert.Throws<ArgumentException>(() => repo.GetTodayForecast("9999"));
}
```

Denne tilgang tester den rigtige `WeatherRepository`-implementering uden nogen afhængighed af live-netværk, og uden at Moq mockes på en konkret klasse.
