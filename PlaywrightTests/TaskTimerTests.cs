using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace PlaywrightTests;

[TestClass]
public partial class TaskTimerTests : PageTest
{
    [TestMethod]
    public async Task TaskTimer_Should_StartAndPause_Correctly()
    {
        await Page.GotoAsync("http://localhost:5085/tasktimer");
        
        // Verify initial state
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Task Timer" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Start" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Resume" })).ToBeVisibleAsync();
        
        // Start the timer
        await Page.GetByRole(AriaRole.Button, new() { Name = "Start" }).ClickAsync();
        await Task.Delay(100); // Wait for timer to tick

        // Verify timer is running
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Pause" })).ToBeVisibleAsync();
        await Expect(Page.Locator("p").First).ToContainTextAsync("Remaining Time: ");
    }
}
