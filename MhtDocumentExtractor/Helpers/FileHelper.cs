using MimeExtractor.Models.Options;

namespace MimeExtractor.Helpers;

public static class FileHelper
{
    internal static bool IsValidFileName(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        var invalidChars = Path.GetInvalidFileNameChars();

        foreach (var c in invalidChars)
        {
            if (fileName.Contains(c))
            {
                return false;
            }
        }

        if (fileName.EndsWith('.') || fileName.EndsWith(' '))
        {
            return false;
        }

        return true;
    }

    internal static string GetReplacementFileName(ApplicationOptions? options, string contentType)
    {
        var extension = GetDefaultExtension(options, contentType);
        var fileName = Path.GetTempFileName();
        fileName = Path.ChangeExtension(fileName, extension);

        return fileName;
    }

    private static string GetDefaultExtension(ApplicationOptions? options, string contentType)
    {
        if (options?.MimeTypeToExtensions is not null && options.MimeTypeToExtensions.TryGetValue(contentType, out var extension))
        {
            return extension;
        }

        return options?.DefaultExtension ?? ".bin";
    }
}

