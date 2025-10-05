using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace PlaywrightTests;

[TestClass]
public partial class TaskTimerTests : PageTest
{
    // test class variables shared across all tests
    const string port = "5085";
    static readonly Uri url = new($"http://localhost:{port}/tasktimer");
    static readonly TimeSpan clickDelayTime = TimeSpan.FromMilliseconds(100);

    public TaskTimerTests()
    {
        // test constants / static objects
        ArgumentException.ThrowIfNullOrWhiteSpace(port);
        ArgumentOutOfRangeException.ThrowIfEqual(int.TryParse(port, out int portNumber), false);
        ArgumentOutOfRangeException.ThrowIfLessThan(portNumber, 1025);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(portNumber, 65536);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(clickDelayTime, TimeSpan.FromSeconds(2));
    }

    [TestInitialize]
    public async Task TestInitialize()
    {
        await Page.GotoAsync(url.AbsoluteUri, new() { WaitUntil = WaitUntilState.DOMContentLoaded });
    }

    [TestMethod]
    public async Task MainNavigation()
    {
        await Expect(Page).ToHaveURLAsync(url.AbsoluteUri);
    }

    [TestMethod]
    public async Task TaskTimer_Should_StartCorrectly()
    {
        // Verify initial state
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Task Timer" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Start" })).ToBeVisibleAsync();
        await Expect(Page.Locator("div[asp-for=timeRemaining]").First).ToContainTextAsync("Time Remaining");

        // Start the timer
        await Page.GetByRole(AriaRole.Button, new() { Name = "Start" }).ClickAsync();

        // Verify timer is running
        await Expect(Page.Locator("#timerDisplay").First).ToContainTextAsync("remaining");
        await Expect(Page.Locator("#timerDisplay").First).ToHaveTextAsync(MatchClockRegex());
    }

    [TestMethod]
    public async Task TaskTimer_Should_StopWhenReloaded()
    {
        // Verify initial state
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Task Timer" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Start" })).ToBeVisibleAsync();
        await Expect(Page.Locator("div[asp-for=timeRemaining]").First).ToContainTextAsync("Time Remaining");

        // Start the timer
        await Page.GetByRole(AriaRole.Button, new() { Name = "Start" }).ClickAsync();

        await Page.ReloadAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Task Timer" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Start" })).ToBeVisibleAsync();
        await Expect(Page.Locator("div[asp-for=timeRemaining]").First).ToContainTextAsync("Time Remaining");

        // Verify timer is stoped
        await Expect(Page.Locator("#timerDisplay").First).Not.ToContainTextAsync("remaining");
        await Expect(Page.Locator("#timerDisplay").First).ToHaveTextAsync(MatchClockRegex());
    }

    [GeneratedRegex(@"\d\d:\d\d")]
    private static partial Regex MatchClockRegex();
}
