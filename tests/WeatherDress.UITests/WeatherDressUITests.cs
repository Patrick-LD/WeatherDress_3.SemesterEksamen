using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using FluentAssertions;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace UITestRESTWeatherDressAsync;

public class WeatherDressUITests
{
    private IWebDriver _driver = null!;
    private const string BaseUrl = "http://localhost:5058";
    private const string ValidPostnummer = "2100";
    private const string InvalidPostnummer = "0000";

    [SetUp]
    public void Setup()
    {
        new DriverManager().SetUpDriver(new ChromeConfig());

        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");

        _driver = new ChromeDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
    }

    [TearDown]
    public void TearDown()
    {
        _driver.Quit();
        _driver.Dispose();
    }

    // --- Forside: postnummer-input ---

    [Test]
    public void Side_HarPostnummerInputfelt()
    {
        _driver.Navigate().GoToUrl(BaseUrl);

        var input = _driver.FindElement(By.Id("postnummer-input"));

        input.Displayed.Should().BeTrue();
    }

    [Test]
    public void Side_HarSendKnap()
    {
        _driver.Navigate().GoToUrl(BaseUrl);

        var knap = _driver.FindElement(By.Id("submit-button"));

        knap.Displayed.Should().BeTrue();
    }

    [Test]
    public void FoerSoegning_ViserIkkeWeatherDressPanel()
    {
        _driver.Navigate().GoToUrl(BaseUrl);

        var panels = _driver.FindElements(By.Id("weatherdress-panel"));

        panels.Should().BeEmpty();
    }

    // --- WeatherDress-panel vises efter gyldig søgning ---

    [Test]
    public void GyldigtPostnummer_ViserWeatherDressPanel()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(ValidPostnummer);

        var panel = VentPaaElement(By.Id("weatherdress-panel"));

        panel.Displayed.Should().BeTrue();
    }

    [Test]
    public void GyldigtPostnummer_ViserVejrIDagSektion()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(ValidPostnummer);

        var sektion = VentPaaElement(By.Id("vejr-i-dag"));

        sektion.Displayed.Should().BeTrue();
    }

    [Test]
    public void GyldigtPostnummer_ViserVejrIGaarSektion()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(ValidPostnummer);

        var sektion = VentPaaElement(By.Id("vejr-i-gaar"));

        sektion.Displayed.Should().BeTrue();
    }

    [Test]
    public void GyldigtPostnummer_ViserTojAnbefaletSektion()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(ValidPostnummer);

        var sektion = VentPaaElement(By.Id("toj-anbefalet"));

        sektion.Displayed.Should().BeTrue();
    }

    [Test]
    public void GyldigtPostnummer_ViserMeddelelseOmkringVejrSektion()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(ValidPostnummer);

        var sektion = VentPaaElement(By.Id("meddelelse-om-vejr"));

        sektion.Displayed.Should().BeTrue();
    }

    // --- Vejr i dag: indhold ---

    [Test]
    public void VejrIDag_IndeholderTemperatur()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(ValidPostnummer);

        var sektion = VentPaaElement(By.Id("vejr-i-dag"));
        var temperatur = sektion.FindElement(By.Id("temperatur"));

        temperatur.Text.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void VejrIDag_IndeholderFugtighed()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(ValidPostnummer);

        var sektion = VentPaaElement(By.Id("vejr-i-dag"));
        var fugtighed = sektion.FindElement(By.Id("fugtighed"));

        fugtighed.Text.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void VejrIDag_IndeholderVejrbeskrivelse()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(ValidPostnummer);

        var sektion = VentPaaElement(By.Id("vejr-i-dag"));
        var beskrivelse = sektion.FindElement(By.Id("vejr-beskrivelse"));

        beskrivelse.Text.Should().NotBeNullOrWhiteSpace();
    }

    // --- Fejlhåndtering ---

    [Test]
    public void UgyldigtPostnummer_ViserFejlbesked()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(InvalidPostnummer);

        var fejl = VentPaaElement(By.Id("error-message"));

        fejl.Displayed.Should().BeTrue();
        fejl.Text.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void UgyldigtPostnummer_ViserIkkeWeatherDressPanel()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(InvalidPostnummer);

        VentPaaElement(By.Id("error-message"));
        var panels = _driver.FindElements(By.Id("weatherdress-panel"));

        panels.Should().BeEmpty();
    }

    // --- Vejr i går: indhold ---

    [Test]
    public void VejrIGaar_IndeholderTemperatur()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(ValidPostnummer);

        var sektion = VentPaaElement(By.Id("vejr-i-gaar"));
        var temperatur = sektion.FindElement(By.Id("temperatur-igaar"));

        temperatur.Text.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void VejrIGaar_IndeholderFugtighed()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(ValidPostnummer);

        var sektion = VentPaaElement(By.Id("vejr-i-gaar"));
        var fugtighed = sektion.FindElement(By.Id("fugtighed-igaar"));

        fugtighed.Text.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void VejrIGaar_IndeholderVejrbeskrivelse()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(ValidPostnummer);

        var sektion = VentPaaElement(By.Id("vejr-i-gaar"));
        var beskrivelse = sektion.FindElement(By.Id("vejr-beskrivelse-igaar"));

        beskrivelse.Text.Should().NotBeNullOrWhiteSpace();
    }

    // --- Vejr i dag: vindstyrke og nedbør ---

    [Test]
    public void VejrIDag_IndeholderVindstyrke()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(ValidPostnummer);

        var sektion = VentPaaElement(By.Id("vejr-i-dag"));
        var vindstyrke = sektion.FindElement(By.Id("vindstyrke"));

        vindstyrke.Text.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void VejrIDag_IndeholderNedbor()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(ValidPostnummer);

        var sektion = VentPaaElement(By.Id("vejr-i-dag"));
        var nedbor = sektion.FindElement(By.Id("nedbor"));

        nedbor.Text.Should().NotBeNullOrWhiteSpace();
    }

    // --- Tøjanbefaling ---

    [Test]
    public void VejrIDag_ViserTojAnbefalinger()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(ValidPostnummer);

        VentPaaElement(By.Id("vejr-i-dag"));
        var tojItems = _driver.FindElements(By.CssSelector("#vejr-i-dag .clothing-panel-item"));

        tojItems.Should().NotBeEmpty();
    }

    [Test]
    public void VejrIDag_TojAnbefalingerHarTekstindhold()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(ValidPostnummer);

        VentPaaElement(By.Id("vejr-i-dag"));
        var foersteItem = _driver.FindElement(By.CssSelector("#vejr-i-dag .clothing-panel-item"));

        foersteItem.Text.Should().NotBeNullOrWhiteSpace();
    }

    // --- Hourly forecast og motor ---

    [Test]
    public void GyldigtPostnummer_ViserHourlyForecast()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(ValidPostnummer);

        var sektion = VentPaaElement(By.Id("hourly-forecast"));

        sektion.Displayed.Should().BeTrue();
    }

    [Test]
    public void GyldigtPostnummer_ViserMotorStyringKnap()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(ValidPostnummer);

        var motor = VentPaaElement(By.Id("motor-styring"));

        motor.Displayed.Should().BeTrue();
    }

    // --- Anbefalingshistorik ---

    [Test]
    public void GyldigtPostnummer_ViserAnbefalingsHistorik()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        IndtastPostnummerOgSend(ValidPostnummer);

        var historik = VentPaaElement(By.Id("anbefaling-historik"));

        historik.Displayed.Should().BeTrue();
    }

    // --- Hjælpemetoder ---

    private void IndtastPostnummerOgSend(string postnummer)
    {
        _driver.FindElement(By.Id("postnummer-input")).SendKeys(postnummer);
        _driver.FindElement(By.Id("submit-button")).Click();
    }

    private IWebElement VentPaaElement(By locator, int sekunder = 10)
    {
        return new WebDriverWait(_driver, TimeSpan.FromSeconds(sekunder))
            .Until(d => d.FindElement(locator));
    }
}
