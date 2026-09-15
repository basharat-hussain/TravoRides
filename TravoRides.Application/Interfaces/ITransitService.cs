using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Package;
using TravoRides.Application.DTOs.PackageRate;
using TravoRides.Application.DTOs.Transit;
using TravoRides.Application.DTOs.TransitRate;

namespace TravoRides.Application.Interfaces
{
    public interface ITransitService
    {
        Task<PagedResponse<TransitDTO>> GetAllAsync(SearchTransitRequest request, CancellationToken cancellationToken = default);
        Task<TransitDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> CreateAsync(CreateTransitRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(UpdateTransitRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<TransitCabRateDTO>> GetCabsWithRatesAsync(Guid transitId, CancellationToken cancellationToken = default);
        Task<TransitCabRateDTO> GetTransitRateAsync(Guid cabId, Guid transitId, CancellationToken cancellationToken);
        Task AddCabsToTransitAsync( Guid packageId,AddCabsToTransitRequest request, CancellationToken cancellationToken = default);
        Task UpdateTransitCabAsync(Guid packageId, Guid cabId, UpdateTransitCabRequest request, CancellationToken cancellationToken = default);

        Task RemoveCabFromTransitAsync(Guid packageId, Guid cabId, CancellationToken cancellationToken = default);
    }
}
