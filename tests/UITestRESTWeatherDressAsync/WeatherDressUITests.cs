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

        input.Should().NotBeNull();
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
