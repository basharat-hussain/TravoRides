using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Package;
using TravoRides.Application.DTOs.Transit;

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

    }
}
