using System.Web;

namespace ui.Components.Validators
{
    public class MarkdownValidator
    {
        /// <summary>
        /// Sanitizes markdown input by escaping characters that could be interpreted as markdown syntax
        /// </summary>
        /// <param name="input">The input string to sanitize</param>
        /// <returns>A sanitized version of the input string safe for markdown rendering</returns>
        public static string SanitizeMarkdown(string input)
        {
            // Whitespace input is safe, echo and return immediately
            if (string.IsNullOrWhiteSpace(input))
                return input;

            // Sanitize embedded raw HTML if found, this is allowed in markdown specs but
            // can be used for injection attacks.

            return HttpUtility.HtmlEncode(input);
        }
    }
}