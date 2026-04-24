# WeatherDress_3.SemesterEksamen
📋 WeatherDress — Projekt setup overblik
Repo: https://github.com/Patrick-LD/WeatherDress_3.SemesterEksamen
Hej team! Dette er en kort guide til hvordan vores repo er sat op, og hvordan vi arbejder sammen i det. Læs det hele igennem — så er vi alle på samme side når vi går i gang med at kode. 🙌

🌿 Sådan arbejder vi med branches (Git Flow)
Vi har to faste branches som altid skal eksistere:

main er vores produktionskode. Kun færdige, stabile releases lander her.
develop er hvor al aktiv udvikling samles. Det er her I starter fra, og det er hit jeres PRs skal merges ind.

Når I skal lave en ny opgave, opretter I en feature branch ud fra develop. Navngivning afhænger af hvad I laver:

feature/<kort-navn> til ny funktionalitet
bugfix/<kort-navn> til fejlrettelser under udvikling
hotfix/<kort-navn> til akutte fixes direkte fra main

Den vigtigste regel: I committer aldrig direkte til develop eller main. Alt skal gennem en Pull Request.
Workflowet for en opgave ser sådan her ud:
bash# Start med at opdatere develop
git checkout develop
git pull origin develop

# Opret din feature branch
git checkout -b feature/min-opgave

# Kod, commit, push
git add .
git commit -m "feat: beskrivelse af ændring"
git push -u origin feature/min-opgave

# Åbn PR på GitHub → develop
# Vent på grøn CI + godkendelse → merge → slet branch

🛡️ Hvorfor din merge nogle gange bliver blokeret
Både main og develop er beskyttede, og det er med vilje. Det betyder at:

Ingen kan pushe direkte til dem — alt skal gennem en PR
CI-pipelinen skal være grøn før I kan merge
Jeres branch skal være up-to-date med target før merge
Force push og sletning er slået fra

Det kan føles lidt bøvlet i starten, men det redder os fra at ødelagt kode ender i hovedbranchene.

🏗️ Projektstrukturen
Solutionen er en standard .NET 9 opsætning:
WeatherDress_3.SemesterEksamen/
├── .github/workflows/ci-pipeline.yml       ← GitHub Actions CI
├── .gitignore
├── README.md
├── WeatherDress.sln
│
├── src/
│   └── WeatherDress.Api/                   ← ASP.NET Core Web API (.NET 9)
│
└── tests/
    ├── WeatherDress.UnitTests/             ← xUnit tests
    └── UITestRESTWeatherDressAsync/        ← NUnit + Selenium UI tests
Vi har tre projekter: selve API'et, et xUnit-projekt til unit tests (med Moq, FluentAssertions og Microsoft.AspNetCore.Mvc.Testing), og et NUnit-projekt til Selenium UI-tests (med Selenium.WebDriver, Support, ChromeDriver, WebDriverManager og FluentAssertions). Navngivningen UITestRESTWeatherDressAsync følger mønsteret fra tidligere semester.

⚙️ CI Pipeline
Pipelinen ligger i .github/workflows/ci-pipeline.yml og kører automatisk når:

Nogen pusher til develop eller main
Nogen åbner en PR mod develop eller main

Den checker koden ud, sætter .NET 9 op, kører dotnet restore, bygger i Release, og kører unit tests. Selenium UI-testene er bevidst ekskluderet indtil videre — de kræver browser-setup i CI som vi sætter op senere.
I kan altid følge med i kørslerne her: https://github.com/Patrick-LD/WeatherDress_3.SemesterEksamen/actions

🚀 Kom i gang
Første gang I joiner repoet:
bashgit clone https://github.com/Patrick-LD/WeatherDress_3.SemesterEksamen.git
cd WeatherDress_3.SemesterEksamen
git checkout develop

dotnet restore
dotnet build
dotnet test tests/WeatherDress.UnitTests
I skal have .NET 9 SDK, Git, Google Chrome (til Selenium) og enten Visual Studio 2022 eller VS Code installeret.

✅ Tjek før I åbner en PR

Koden bygger lokalt med dotnet build
Unit tests passerer lokalt med dotnet test
Commit-beskeder er forståelige
PR'en targeter develop — ikke main
Titel og beskrivelse forklarer hvad og hvorfor
CI er grøn ✅


📝 Commit-beskeder
Vi bruger simple prefixes så man kan skimme git-historikken hurtigt:

feat: — ny funktionalitet
fix: — fejlrettelse
test: — tests tilføjet eller ændret
docs: — dokumentation, fx README
refactor: — omskrivning uden funktionsændring
chore: — vedligehold, dependencies osv.
