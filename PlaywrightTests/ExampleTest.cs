using System.Text.RegularExpressions;
using Microsoft.Playwright.MSTest;

namespace PlaywrightTests;

[TestClass]
public partial class ExampleTest : PageTest
{
    [TestMethod]
    public async Task Home_Page_Should_Display_Welcome_Message()
    {
        // Create a new context and page instance for this test
        // Navigate to the home page
        await Page.GotoAsync("http://localhost:5085");

        // Check the content is displayed correctly
        await Expect(Page).ToHaveTitleAsync(MyRegex());
    }

    [GeneratedRegex("Home")]
    private static partial Regex MyRegex();
}