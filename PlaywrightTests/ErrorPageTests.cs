using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using PlaywrightTests.Helpers;

namespace PlaywrightTests;

[TestClass]
public partial class ErrorPageTests : PageTest
{
    readonly string port = PageTestDefaults.DefaultPort;
    static readonly Uri url = new($"{PageTestDefaults.DefaultBaseUrl}/Error");
    static readonly TimeSpan clickDelayTime = TimeSpan.FromMilliseconds(PageTestDefaults.DefaultTimeout);

    public ErrorPageTests()
    {
        // test constants / static objects
        ArgumentException.ThrowIfNullOrWhiteSpace(port);
        ArgumentOutOfRangeException.ThrowIfEqual(int.TryParse(port, out int portNumber), false);
        ArgumentOutOfRangeException.ThrowIfLessThan(portNumber, 1025);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(portNumber, 65536);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(clickDelayTime, TimeSpan.FromSeconds(2));
    }

    [TestMethod]
    public async Task ErrorPage_Should_ShowRequestId_WhenAvailable()
    {
        await Page.GotoAsync(url.AbsoluteUri);

        await Expect(Page.Locator("h1").First).ToHaveTextAsync("Error.");
        await Expect(Page.Locator("h2").First).ToContainTextAsync("An error occurred");
        await Expect(Page.Locator("h3").First).ToHaveTextAsync("Development Mode");

        var errorMessage = await Page.QuerySelectorAsync("h2");
        Assert.IsNotNull(errorMessage);
    }

    [TestMethod]
    public async Task ErrorPage_Should_DisplayDevelopmentModeInstructions()
    {
        await Page.GotoAsync(url.AbsoluteUri);

        await Expect(Page.Locator("body > div.page > main > article > p:nth-child(5)"))
            .ToContainTextAsync("Swapping to Development environment will display more detailed information about the error that occurred.");
    }

    [TestMethod]
    public async Task ErrorPage_Should_HaveCorrectTitle()
    {
        await Page.GotoAsync(url.AbsoluteUri);

        var title = await Page.TitleAsync();
        Assert.AreEqual("Error", title);
    }

    [TestMethod]
    public async Task ErrorPage_Should_RenderWithoutJavaScriptErrors()
    {
        var consoleMessages = new List<IConsoleMessage>();
        Page.Console += (_, e) => consoleMessages.Add(e);

        await Page.GotoAsync(url.AbsoluteUri);

        Assert.AreEqual(0, consoleMessages.Where(m => m.Type == "error").Count());
    }
}
