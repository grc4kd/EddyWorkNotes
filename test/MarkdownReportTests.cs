using System.Text;
using ui.Components.Validators;

namespace test;

public class MarkdownReportTests
{
    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    public void MarkdownReport_EmptyInputs_PassThruValidator(string? input)
    {
        // Given / When
        string sanitized = MarkdownValidator.SanitizeMarkdown(input!);

        // Then
        Assert.Equal(input, sanitized);
    }

    [Fact]
    public void MarkdownReport_HTMLInput_IsSanitizedBeforeRendering()
    {
        // Given
        // Setup test data with potential markdown special characters
        StringBuilder sb = new();
        sb.Append("### Test Header");           // safe input
        sb.Append("hello <a name=\"n\" href=\"javascript:alert('xss')\">*you*</a>");   // not safe - invokes JS function

        string input = sb.ToString();
        string sanitized = string.Empty;

        if (input != null)
        {
            sanitized = MarkdownValidator.SanitizeMarkdown(input);
        }

        // Assert - Verify markdown is properly sanitized and escaped
        Assert.Contains("# Test Header", sanitized);
        Assert.Contains("hello &lt;a name=&quot;n&q", sanitized);
    }
}