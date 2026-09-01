using TravoRides.Application.DTOs.Package;
using TravoRides.Application.DTOs.Common;

namespace TravoRides.Application.Interfaces
{
    public interface IPackageService
    {
        Task<PagedResponse<PackageDTO>> GetAllAsync(int pageNumber, int pageSize, string? keyword, CancellationToken cancellationToken = default);
        Task<PackageDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> CreateAsync(CreatePackageRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(PackageDTO request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
