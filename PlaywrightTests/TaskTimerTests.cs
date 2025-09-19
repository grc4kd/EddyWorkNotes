using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace PlaywrightTests;

[TestClass]
public partial class TaskTimerTests : PageTest
{
    [TestInitialize]
    public async Task TestInitialize()
    {
        await Page.GotoAsync("http://localhost:5085/tasktimer");
    }

    [TestMethod]
    public async Task MainNavigation()
    {
        await Expect(Page).ToHaveURLAsync("http://localhost:5085/tasktimer");
    }

    [TestMethod]
    public async Task TaskTimer_Should_StartAndPause_Correctly()
    {
        // Verify initial state
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Task Timer" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Start" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Resume" })).ToBeVisibleAsync();

        // Click the start button
        await Page.GetByRole(AriaRole.Button, new() { Name = "Start" }).ClickAsync();

        // Verify timer is running
        await Expect(Page.Locator("p").First).ToContainTextAsync("Remaining Time: ");
    }
}
