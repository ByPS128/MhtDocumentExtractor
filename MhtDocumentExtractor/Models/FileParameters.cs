namespace MimeExtractor.Models;

internal sealed class FileParameters() : Dictionary<string, FileParameterAttributes>(StringComparer.OrdinalIgnoreCase)
{
    public string? GetBoundaryValue()
    {
        return TryGetValue(Cosntants.HttpHeaders.ContentType, out var contentType) ? contentType.Attributes.GetValueOrDefault(Cosntants.Attributes.Boundary) : null;
    }
}
