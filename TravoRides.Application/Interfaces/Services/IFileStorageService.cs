
using TravoRides.Application.Common.Models;

namespace TravoRides.Application.Interfaces.Services
{
    public interface IFileStorageService
    {

        Task<FileUploadResult> UploadAsync(FileUploadRequest request, CancellationToken cancellationToken = default);

        Task<Stream> DownloadAsync(string relativePath, CancellationToken cancellationToken = default);

        Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(string relativePath, CancellationToken cancellationToken = default);
    }
}
