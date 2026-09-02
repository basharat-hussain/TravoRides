using TravoRides.Application.DTOs.Package;
using TravoRides.Application.DTOs.Common;

namespace TravoRides.Application.Interfaces
{
    public interface IPackageService
    {
        Task<PagedResponse<PackageDTO>> GetAllAsync(SearchPackageRequest request, CancellationToken cancellationToken = default);
        Task<PackageDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> CreateAsync(CreatePackageRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(UpdatePackageRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
