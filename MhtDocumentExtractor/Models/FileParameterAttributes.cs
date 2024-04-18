namespace MimeExtractor.Models;

internal sealed class FileParameterAttributes
{
    public FileParameterAttributes(string value)
    {
        Value = value;
        Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    public string Value { get; }

    public Dictionary<string, string> Attributes { get; }
}
