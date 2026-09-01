using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRiders.Application.Common.Options
{
    public class FileStorageOptions
    {
        public string RootFolder { get; set; } = string.Empty;
        public long MaxFileSizeInBytes { get; set; }
        public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
        public string[] AllowedContentTypes { get; set; } = Array.Empty<string>();
    }
}
