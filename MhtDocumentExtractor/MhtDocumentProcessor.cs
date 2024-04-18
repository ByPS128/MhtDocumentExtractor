using MimeExtractor.Models;
using MimeExtractor.Models.Options;

namespace MimeExtractor;

public sealed class MhtDocumentProcessor
{
    private readonly Boundaries _boundaries = [];
    private readonly ApplicationOptions? _options;

    public MhtDocumentProcessor(ApplicationOptions? options)
    {
        _options = options;
    }
    public async Task Extract(string sourceMhtDocumentFileName, string outputDirectory)
    {
        var content = await File.ReadAllLinesAsync(sourceMhtDocumentFileName);
        var index = 0;
        var document = new MhtDocument();

        var headerBlock = GetHeaderBlock(ref content, ref index);
        var rootBoundaryFound = ReadAndAddNewBoundary(_boundaries, ref headerBlock);
        var bodyBlock = GetBodyBlock(ref content, ref index);
        while (rootBoundaryFound && index < content.Length)
        {
            headerBlock = GetHeaderBlock(ref content, ref index);
            var itemParams = ParseBoundaryParameters(ref headerBlock);
            if (itemParams.TryGetValue(Cosntants.HttpHeaders.ContentType, out var contentType)
                && string.Equals(contentType!.Value, Cosntants.MimeTypes.MultipartAlternative, StringComparison.OrdinalIgnoreCase)
                && contentType.Attributes.TryGetValue(Cosntants.Attributes.Boundary, out var boundary))
            {
                _boundaries.Add(boundary);
                index++;
                continue;
            }

            bodyBlock = GetBodyBlock(ref content, ref index);
            document.AddItem(new DocumentFile(_options, itemParams, bodyBlock, outputDirectory));
        }

        document.StoreFiles();
    }

    private static string[] GetHeaderBlock(ref string[] content, ref int index)
    {
        var result = new List<string>();
        while (index < content.Length)
        {
            var line = GetLine(ref content, ref index);
            if (string.IsNullOrEmpty(line))
            {
                break;
            }

            result.Add(line);
        }

        return [.. result];
    }

    private string[] GetBodyBlock(ref string[] content, ref int index)
    {
        var result = new List<string>();
        while (index < content.Length)
        {
            if (IsNextLineBoundaryMark(ref content, index))
            {
                index++;
                break;
            }

            var line = GetLine(ref content, ref index);
            result.Add(line!);
        }

        return [.. result];
    }

    private bool IsNextLineBoundaryMark(ref string[] block, int index)
    {
        if (index + 1 < block.Length)
        {
            var line = block[index + 1];
            return _boundaries.IsLineBoundary(line);
        }

        return false;
    }

    private static FileParameters ParseBoundaryParameters(ref string[] block)
    {
        var index = 0;
        var result = new FileParameters();
        while (index < block.Length)
        {
            var line = block[index++];
            if (string.IsNullOrEmpty(line))
            {
                break;
            }

            var hasParameter = TryParseParameter(line, ':', out var key, out var value);
            if (!hasParameter)
            {
                continue;
            }

            var parameterValue = new FileParameterAttributes(value);
            result[key.Trim()] = parameterValue;

            ParseBoundaryParameterAttributes(parameterValue, ref block, ref index);
        }

        return result;
    }

    private static void ParseBoundaryParameterAttributes(FileParameterAttributes fileParameterAttributes, ref string[] block, ref int index)
    {
        while (index < block.Length)
        {
            var line = block[index];
            if (string.IsNullOrEmpty(line) || (line.Length > 1 && line[0] != '\t'))
            {
                break;
            }

            var hasAttribute = TryParseParameter(line, '=', out var key, out var value);
            if (hasAttribute)
            {
                fileParameterAttributes.Attributes[key.Trim()] = value;
                index++;
                continue;
            }

            break;
        }
    }

    private static bool ReadAndAddNewBoundary(Boundaries boundaries, ref string[] block)
    {
        var newItem = ParseBoundaryParameters(ref block);
        var value = newItem.GetBoundaryValue();
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        boundaries.Add(value);

        return true;
    }

    private static string? GetLine(ref string[] content, ref int index)
    {
        while (index < content.Length - 1)
        {
            var str = content[index];
            if (string.IsNullOrEmpty(str))
            {
                index++;
                return str;
            }

            index += 2;
            return str;
        }

        return null;
    }

    private static bool TryParseParameter(string line, char delimiter, out string key, out string value)
    {
        var index = line.IndexOf(delimiter);
        if (index <= 0)
        {
            key = string.Empty;
            value = string.Empty;

            return false;
        }

        key = line[..index];
        var rawValue = line[(index + 1)..].Trim();
        if (rawValue[^1] == ';')
        {
            rawValue = rawValue[..^1];
        }

        value = rawValue.Trim().Trim('"');

        return true;
    }
}
