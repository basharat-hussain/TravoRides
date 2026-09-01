using TravoRides.Application.DTOs.CabFeatures;
using TravoRides.Application.DTOs.Common;

namespace TravoRides.Application.Interfaces
{
    public interface ICabFeaturesService
    {
        Task<PagedResponse<CabFeaturesDTO>> GetAllAsync(int pageNumber, int pageSize, string? keyword, CancellationToken cancellationToken = default);
        Task<CabFeaturesDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> CreateAsync(CreateCabFeaturesRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(UpdateCabFeaturesRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
