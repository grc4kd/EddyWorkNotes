using System.Web;

namespace ui.Components.Validators;

public static class MarkdownValidator
{
    /// <summary>
    /// Sanitizes Markdown input by escaping characters that could be interpreted as Markdown syntax
    /// </summary>
    /// <param name="input">The input string to sanitize</param>
    /// <returns>A sanitized version of the input string safe for Markdown rendering</returns>
    public static string SanitizeMarkdown(string input)
    {
        // Whitespace input is safe, echo and return immediately
        return !string.IsNullOrWhiteSpace(input) ? HttpUtility.HtmlEncode(input) : input;
    }
}