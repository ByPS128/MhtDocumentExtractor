namespace MimeExtractor;

public static class Cosntants
{
    public const string DefaultContentType = MimeTypes.ApplicationOctetStream;

    public static class Attributes
    {
        public const string Boundary = "boundary";
    }

    public static class MimeTypes
    {
        public const string TextHtml = "text/html";
        public const string MultipartAlternative = "multipart/alternative";
        public const string MultipartRelated = "multipart/related";
        public const string ApplicationOctetStream = "application/octet-stream";
        public const string QuotedPrintable = "quoted-printable";
        public const string SevenBit = "7bit";
        public const string Base64 = "base64";
    }

    public static class HttpHeaders
    {
        public const string ContentType = "Content-Type";
        public const string ContentLocation = "Content-Location";
        public const string ContentTransferEncoding = "Content-Transfer-Encoding";
    }
}
