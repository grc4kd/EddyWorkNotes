using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace PlaywrightTests;

[TestClass]
public partial class TaskTimerTests : PageTest
{
    [TestInitialize]
    public async Task TestInitialize()
    {
        await Page.GotoAsync("http://localhost:5085/tasktimer", new() { WaitUntil = WaitUntilState.DOMContentLoaded });
    }

    [TestMethod]
    public async Task MainNavigation()
    {
        await Expect(Page).ToHaveURLAsync("http://localhost:5085/tasktimer");
    }

    [TestMethod]
    public async Task TaskTimer_Should_StartCorrectly()
    {
        // Verify initial state
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Task Timer" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Start" })).ToBeVisibleAsync();
        await Expect(Page.Locator("main > article > div div.mb-3[asp-for=timeRemaining]").First).ToContainTextAsync("Time Remaining");

        // Start the timer
        await Page.GetByRole(AriaRole.Button, new() { Name = "Start" }).ClickAsync();
        await Task.Delay(10); // Wait for timer to tick

        // Verify timer is running
        await Expect(Page.Locator("#timerDisplay").First).ToContainTextAsync("remaining");
    }
}
