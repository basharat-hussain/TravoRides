
using TravoRiders.Application.Common.Models;

namespace TravoRiders.Application.Interfaces.Services
{
    public interface IFileStorageService
    {

        Task<FileUploadResult> UploadAsync(FileUploadRequest request, CancellationToken cancellationToken = default);

        Task<Stream> DownloadAsync(string relativePath, CancellationToken cancellationToken = default);

        Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(string relativePath, CancellationToken cancellationToken = default);
    }
}
