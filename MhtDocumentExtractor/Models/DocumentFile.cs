using System.Text;
using MimeExtractor.Helpers;
using MimeExtractor.Models.Options;
using MimeKit.Encodings;

namespace MimeExtractor.Models;

internal sealed class DocumentFile
{
    private readonly ApplicationOptions? _options;

    public DocumentFile(ApplicationOptions? options, FileParameters fileParams, string[] bodyBlock, string outputDirectory)
    {
        _options = options;
        FileParams = fileParams;
        BodyBlock = bodyBlock;
        OutputDirectory = outputDirectory;
    }

    public string OutputDirectory { get; }

    public FileParameters FileParams { get; }

    public string[] BodyBlock { get; }

    public string ReplacementFileName { get; private set; } = null!;

    public string SourceLink { get; private set; } = null!;

    public string ReplacementLink { get; private set; }

    public void SetReplacementFileName(string fullFileName)
    {
        ReplacementFileName = fullFileName;
    }

    public void SetSourceLink(string sourceLink)
    {
        SourceLink = sourceLink;
    }

    internal void StoreContent(Dictionary<string, string> filesRedirect)
    {
        var encoding = FileParams[Cosntants.HttpHeaders.ContentTransferEncoding].Value;
        var contentType = FileParams[Cosntants.HttpHeaders.ContentType].Value;

        if (string.Equals(encoding, Cosntants.MimeTypes.QuotedPrintable, StringComparison.OrdinalIgnoreCase))
        {
            var fileContent = ConvertQuotedStringToString(BodyBlock, contentType);
            ResolveFilesRedirections(fileContent, filesRedirect);
            File.WriteAllText(ReplacementFileName, fileContent.ToString());
        }
        else if (string.Equals(encoding, Cosntants.MimeTypes.SevenBit, StringComparison.OrdinalIgnoreCase))
        {
            var fileContent = ListToStringBuilder(BodyBlock, Environment.NewLine);
            ResolveFilesRedirections(fileContent, filesRedirect);
            File.WriteAllText(ReplacementFileName, fileContent.ToString());
        }
        else if (string.Equals(encoding, Cosntants.MimeTypes.Base64, StringComparison.OrdinalIgnoreCase))
        {
            var fileContent = ListToStringBuilder(BodyBlock).ToString();
            var bytes = Convert.FromBase64String(fileContent);
            File.WriteAllBytes(ReplacementFileName, bytes);
        }
        else
        {
            Console.WriteLine($"Unrecognized encoding: '{encoding}'");
            return;
        }

        Console.WriteLine($"{encoding}: {ReplacementFileName}");
    }

    private void ResolveFilesRedirections(StringBuilder fileContent, Dictionary<string, string> filesRedirect)
    {
        foreach (var pair in filesRedirect)
        {
            fileContent.Replace(pair.Key, pair.Value);
        }
    }

    private StringBuilder ListToStringBuilder(string[] block, string? appendAfterEachLine = null)
    {
        var insertAfterLine = appendAfterEachLine is not null;
        var sb = new StringBuilder();
        foreach (var line in block)
        {
            sb.Append(line);
            if (insertAfterLine)
            {
                sb.Append(appendAfterEachLine);
            }
        }

        return sb;
    }

    private StringBuilder ConvertQuotedStringToString(string[] quotedString, string contentType)
    {
        var sb = new StringBuilder();
        if (string.Equals(contentType, Cosntants.MimeTypes.ApplicationOctetStream, StringComparison.OrdinalIgnoreCase))
        {
            foreach (var input in quotedString)
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var decoder = new QuotedPrintableDecoder();

                var output = new byte[decoder.EstimateOutputLength(input.Length)];
                var outputLength = decoder.Decode(inputBytes, 0, input.Length, output);

                var result = Encoding.UTF8.GetString(output, 0, outputLength);
                sb.Append(result);
            }
        }
        else if (string.Equals(contentType, Cosntants.MimeTypes.TextHtml, StringComparison.OrdinalIgnoreCase))
        {
            foreach (var input in quotedString)
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var decoder = new QuotedPrintableDecoder();

                var output = new byte[decoder.EstimateOutputLength(input.Length)];
                var outputLength = decoder.Decode(inputBytes, 0, input.Length, output);

                var result = Encoding.UTF8.GetString(output, 0, outputLength);
                sb.Append(result);
                if (input.Length >= 1 && input[^1] != '=')
                {
                    sb.Append(Environment.NewLine);
                }
            }
        }

        return sb;
    }

    public string GetContentLocation()
    {
        return FileParams[Cosntants.HttpHeaders.ContentLocation].Value;
    }

    public string GetContentType()
    {
        if (FileParams.TryGetValue(Cosntants.HttpHeaders.ContentType, out var value))
        {
            return value.Value;
        }

        return Cosntants.DefaultContentType;
    }

    public void CalculateLinksAndFileNames()
    {
        var documentOriginLocation = GetContentLocation();
        var fileNameFromLocation = Path.GetFileName(documentOriginLocation);
        var fileName = FileHelper.IsValidFileName(fileNameFromLocation) ? fileNameFromLocation : FileHelper.GetReplacementFileName(_options, GetContentType());
        var fileNamePath = Path.Combine(OutputDirectory, fileName);

        SetSourceLink(documentOriginLocation);
        SetReplacementFileName(fileNamePath);
        SetReplacementLink(fileNamePath);
    }

    private void SetReplacementLink(string fileNamePath)
    {
        ReplacementLink = $"file://{fileNamePath.Replace('\\', '/')}";
    }
}
