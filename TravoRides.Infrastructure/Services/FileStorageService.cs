using Microsoft.Extensions.Options;
using TravoRiders.Application.Common.Exceptions;
using TravoRiders.Application.Common.Models;
using TravoRiders.Application.Common.Options;
using TravoRiders.Application.Interfaces.Services;

namespace AlArwaSolutions.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly FileStorageOptions _options;

        public FileStorageService(IOptions<FileStorageOptions> options)
        {
            _options = options.Value;
        }

        public async Task<FileUploadResult> UploadAsync(FileUploadRequest request, CancellationToken cancellationToken = default)
        {
            ValidateFile(request);

            var extension = Path.GetExtension(request.FileName);

            var storedFileName = $"{Guid.NewGuid()}{extension}";

            var folder = Path.Combine(
                _options.RootFolder,
                request.FolderName);

            Directory.CreateDirectory(folder);

            var fullPath = Path.Combine(folder, storedFileName);

            await using var destination =
                new FileStream(
                    fullPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None);

            await request.Stream.CopyToAsync(
                destination,
                cancellationToken);

            return new FileUploadResult
            {
                FileName = request.FileName,
                StoredFileName = storedFileName,
                RelativePath = Path.Combine(
                    _options.RootFolder,
                    request.FolderName,
                    storedFileName).Replace("\\", "/"),
                AbsolutePath = fullPath,
                FileSize = destination.Length,
                ContentType = request.ContentType
            };
        }

        public async Task<Stream> DownloadAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            var fullPath = Path.Combine(
                _options.RootFolder,
                relativePath);

            if (!File.Exists(fullPath))
                throw new ResourceNotFoundException("File not found.");

            var memory = new MemoryStream();

            await using var stream =
                File.OpenRead(fullPath);

            await stream.CopyToAsync(memory, cancellationToken);

            memory.Position = 0;

            return memory;
        }

        public Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            var fullPath = Path.Combine(
                _options.RootFolder,
                relativePath);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            var fullPath = Path.Combine(
                _options.RootFolder,
                relativePath);

            return Task.FromResult(File.Exists(fullPath));
        }

        private void ValidateFile(FileUploadRequest request)
        {
            if (request.Stream == null)
                throw new ValidationException("File is required.");

            if (request.Stream.Length == 0)
                throw new ValidationException("File is empty.");

            if (request.Stream.Length > _options.MaxFileSizeInBytes)
                throw new ValidationException(
                    $"Maximum allowed file size is {_options.MaxFileSizeInBytes / (1024 * 1024)} MB.");

            var extension = Path.GetExtension(request.FileName);

            if (!_options.AllowedExtensions.Any(x =>
                    x.Equals(extension, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationException(
                    $"File type '{extension}' is not allowed.");
            }

            if (!_options.AllowedContentTypes.Any(x =>
                    x.Equals(request.ContentType, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationException(
                    $"Content type '{request.ContentType}' is not allowed.");
            }
        }
    }
}
