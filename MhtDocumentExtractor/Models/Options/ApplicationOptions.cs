namespace MimeExtractor.Models.Options;

public sealed class ApplicationOptions
{
    public const string ConfigKey = "ApplicationOptions";

    public string? DefaultExtension { get; set; }

    public Dictionary<string, string> MimeTypeToExtensions { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}
