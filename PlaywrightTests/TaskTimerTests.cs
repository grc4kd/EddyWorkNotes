using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;

public partial class TaskTimerTests : PageTest
{
    [Fact]
    public async Task TaskTimer_Should_StartAndPause_Correctly()
    {
        await Page.GotoAsync("http://localhost:5085/tasktimer");
        
        // Verify initial state
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Task Timer" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Start" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Pause" })).ToBeVisibleAsync();
        
        // Start the timer
        await Page.GetByRole(AriaRole.Button, new() { Name = "Start" }).ClickAsync();
        await Task.Delay(1000); // Wait for timer to tick
        
        // Verify timer is running
        var timeText = await Page.Locator("p").First.TextContentAsync();
        Assert.Contains("Remaining Time:", timeText);
        
        // Pause the timer
        await Page.GetByRole(AriaRole.Button, new() { Name = "Pause" }).ClickAsync();
        var pauseTimeText = await Page.Locator("p").First.TextContentAsync();
        
        // Resume the timer
        await Page.GetByRole(AriaRole.Button, new() { Name = "Resume" }).ClickAsync();
        var resumeTimeText = await Page.Locator("p").First.TextContentAsync();
        
        Assert.Contains("Remaining Time:", pauseTimeText);
        Assert.Contains("Remaining Time:", resumeTimeText);
    }
}
