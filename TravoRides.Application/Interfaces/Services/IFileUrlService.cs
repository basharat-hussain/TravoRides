namespace TravoRides.Application.Interfaces.Services
{
    /// <summary>
    /// Service for converting relative file paths to absolute URLs
    /// </summary>
    public interface IFileUrlService
    {
        /// <summary>
        /// Converts a relative file path to an absolute URL based on the current HTTP request context
        /// </summary>
        /// <param name="relativePath">The relative path to the file (e.g., "uploads/clients/guid.jpg")</param>
        /// <returns>The absolute URL (e.g., "https://api.example.com/uploads/clients/guid.jpg")</returns>
        string GetAbsoluteUrl(string? relativePath);

        /// <summary>
        /// Gets the base URL of the API (e.g., "https://api.example.com")
        /// </summary>
        /// <returns>The base URL including protocol and domain</returns>
        string GetBaseUrl();
    }
}
