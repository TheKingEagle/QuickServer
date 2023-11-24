using System.Text.RegularExpressions;

namespace QuickServerDemo
{
    internal class InputSanitizer
    {

        internal static string Sanitize(string text)
        {
            List<string> allowedTags = new List<string> { "b", "strong", "em","ul","ol","li" };
            string pattern = @"<script[^>]*>[\s\S]*?</script>";
            string sanitizedInput = Regex.Replace(text, pattern, "", RegexOptions.IgnoreCase);
            pattern = "<[^>]+>";
            sanitizedInput = Regex.Replace(sanitizedInput, pattern, "");
            sanitizedInput = System.Net.WebUtility.HtmlDecode(sanitizedInput);
            foreach (var tag in allowedTags)
            {
                sanitizedInput = sanitizedInput.Replace($"[ALLOWED_{tag}]", $"<{tag}>");
            }

            return sanitizedInput;
        }
    }
}
