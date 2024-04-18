namespace MimeExtractor.Models;

internal sealed class MhtDocument
{
    private readonly Dictionary<string, string> _filesNamesRegister = [];

    private readonly List<DocumentFile> _containedFiles = [];

    public void AddItem(DocumentFile documentFile)
    {
        _containedFiles.Add(documentFile);
        documentFile.CalculateLinksAndFileNames();
        _filesNamesRegister[documentFile.SourceLink] = documentFile.ReplacementLink;
    }

    public void StoreFiles()
    {
        foreach (var item in _containedFiles)
        {
            item.StoreContent(_filesNamesRegister);
        }
    }
}
