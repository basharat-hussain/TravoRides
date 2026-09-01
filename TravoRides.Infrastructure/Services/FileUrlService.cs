using Microsoft.AspNetCore.Http;
using TravoRiders.Application.Interfaces.Services;

namespace AlArwaSolutions.Infrastructure.Services
{
    public class FileUrlService : IFileUrlService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileUrlService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the base URL of the current HTTP request
        /// </summary>
        public string GetBaseUrl()
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
                return string.Empty;

            var request = context.Request;
            var scheme = request.Scheme;
            var host = request.Host.Host;
            var port = request.Host.Port;

            // Construct base URL based on scheme and host
            if (port.HasValue && !IsDefaultPort(scheme, port.Value))
            {
                return $"{scheme}://{host}:{port}";
            }

            return $"{scheme}://{host}";
        }

        /// <summary>
        /// Converts a relative file path to an absolute URL
        /// </summary>
        public string GetAbsoluteUrl(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return string.Empty;

            var baseUrl = GetBaseUrl();

            if (string.IsNullOrWhiteSpace(baseUrl))
                return relativePath;

            // Normalize the relative path (remove leading slashes)
            var normalizedPath = relativePath.TrimStart('/', '\\');

            // Replace backslashes with forward slashes for URL compatibility
            normalizedPath = normalizedPath.Replace("\\", "/");

            return $"{baseUrl}/{normalizedPath}";
        }

        /// <summary>
        /// Checks if a port is a default port for the given scheme
        /// </summary>
        private bool IsDefaultPort(string scheme, int port)
        {
            return (scheme == "http" && port == 80) || (scheme == "https" && port == 443);
        }
    }
}
