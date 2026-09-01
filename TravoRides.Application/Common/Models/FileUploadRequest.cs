namespace TravoRiders.Application.Common.Models
{
    public sealed class FileUploadRequest
    {
        public Stream Stream { get; init; } = Stream.Null;

        public string FileName { get; init; } = string.Empty;

        public string ContentType { get; init; } = string.Empty;

        public string FolderName { get; init; } = string.Empty;
    }
}
