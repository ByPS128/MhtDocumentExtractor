namespace MimeExtractor.Models;

internal class Boundaries : List<string>
{
    public bool IsLineBoundary(string? line)
    {
        return string.IsNullOrEmpty(line) is false && line.Length > 2 && line.StartsWith("--") && Contains(line.Substring(2));
    }
}
