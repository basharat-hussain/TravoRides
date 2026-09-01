namespace TravoRiders.Application.Common.Models
{
    public class FileUploadResult
    {
        public string FileName { get; set; } = string.Empty;

        public string StoredFileName { get; set; } = string.Empty;

        public string AbsolutePath { get; set; } = string.Empty;
        public string RelativePath { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public string ContentType { get; set; } = string.Empty;
    }
}
