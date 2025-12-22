using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace PlaywrightTests;

[TestClass]
public partial class TaskTimerTests : PageTest
{
    // test class variables shared across all tests
    private const string Port = "5085";
    private static readonly Uri Url = new($"http://localhost:{Port}/tasktimer");

    public TaskTimerTests()
    {
        // test constants / static objects
        ArgumentException.ThrowIfNullOrWhiteSpace(Port);
        ArgumentOutOfRangeException.ThrowIfEqual(int.TryParse(Port, out var portNumber), false);
        ArgumentOutOfRangeException.ThrowIfLessThan(portNumber, 1025);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(portNumber, 65536);
    }

    [TestInitialize]
    public async Task TestInitialize()
    {
        await Page.GotoAsync(Url.AbsoluteUri, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
    }

    [TestMethod]
    public async Task MainNavigation()
    {
        var builder = Page.Url.StartsWith("https")
            ? new UriBuilder("https", Url.Host, 7067, Url.PathAndQuery)
            : new UriBuilder(Url.AbsoluteUri);

        await Expect(Page).ToHaveURLAsync(builder.Uri.AbsoluteUri);
    }

    [TestMethod]
    public async Task TaskTimer_Should_StartCorrectly()
    {
        // Verify initial state
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Task Timer" }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Start" })).ToBeVisibleAsync();
        await Expect(Page.Locator("div[asp-for=timeRemaining]").First).ToContainTextAsync("Time Remaining");

        // Start the timer
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Start" }).ClickAsync();

        // Verify timer is running
        await Expect(Page.Locator("#timerDisplay").First).ToContainTextAsync("remaining");
        await Expect(Page.Locator("#timerDisplay").First).ToHaveTextAsync(MatchClockRegex());
    }

    [TestMethod]
    public async Task TaskTimer_Should_StopWhenReloaded()
    {
        // Verify initial state
        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Task Timer" }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Start" })).ToBeVisibleAsync();
        await Expect(Page.Locator("div[asp-for=timeRemaining]").First).ToContainTextAsync("Time Remaining");

        // Start the timer
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Start" }).ClickAsync();

        await Page.ReloadAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Task Timer" }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Start" })).ToBeVisibleAsync();
        await Expect(Page.Locator("div[asp-for=timeRemaining]").First).ToContainTextAsync("Time Remaining");

        // Verify timer is stopped
        await Expect(Page.Locator("#timerDisplay").First).ToHaveTextAsync("00:00 remaining");
    }

    [TestMethod]
    public async Task TaskTimer_Should_ResetOnNavigation()
    {
        await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Task Timer" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Report - Work Notes" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Task Timer" }).ClickAsync();

        // Verify timer is not running
        await Expect(Page.Locator("#timerDisplay").First).ToContainTextAsync("00:00 remaining");

        // start a new timer
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Start" }).ClickAsync();

        // Verify page state
        await Expect(Page.Locator("div.card.container-md p").First).ToContainTextAsync("Current Phase");
        await Expect(Page.Locator("div.card.container-md p").First).ToContainTextAsync("Work");
        await Expect(Page.Locator("#timerDisplay").First).ToHaveTextAsync(MatchClockRegex());

        // Disconnect (stops the background service timer)
        await Page.CloseAsync();
    }

    [GeneratedRegex(@"\d\d:\d\d")]
    private static partial Regex MatchClockRegex();
}