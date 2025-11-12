using Microsoft.Playwright.MSTest;
using PlaywrightTests.Helpers;

namespace PlaywrightTests;

[TestClass]
public class ContactPageTests : PageTest
{
    static readonly Uri url = new($"{PageTestDefaults.DefaultBaseUrl}/Contact");

    [TestMethod]
    public async Task ContactPage_Should_DisplayContactList()
    {
        // Arrange
        var page = await Browser.NewPageAsync();

        // Act
        await page.GotoAsync(url.AbsoluteUri);

        // Assert
        await page.WaitForSelectorAsync("h1");
        var title = await page.TitleAsync();
        Assert.IsTrue(title.Contains("Contact"));
    }
}