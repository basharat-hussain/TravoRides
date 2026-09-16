using TravoRides.Application.DTOs.Package;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.PackageRate;

namespace TravoRides.Application.Interfaces
{
    public interface IPackageService
    {
        Task<PagedResponse<PackageDTO>> GetAllAsync(SearchPackageRequest request, CancellationToken cancellationToken = default);
        Task<PackageDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> CreateAsync(CreatePackageRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(UpdatePackageRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<PackageCabRateDTO>> GetCabsWithRatesAsync( Guid packageId, CancellationToken cancellationToken = default);
        Task<PackageCabRateDTO> GetPackageRateAsync(Guid cabId, Guid transitId, CancellationToken cancellationToken);
        Task AddCabsToPackageAsync( Guid packageId,PackageCabRequest request, CancellationToken cancellationToken = default);
        Task UpdatePackageCabAsync( Guid packageId, Guid cabId, UpdatePackageCabRequest request,CancellationToken cancellationToken = default);

        Task RemoveCabFromPackageAsync( Guid packageId, Guid cabId,CancellationToken cancellationToken = default);
    }
}
