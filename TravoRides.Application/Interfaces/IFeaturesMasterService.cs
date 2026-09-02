using TravoRides.Application.DTOs.FeaturesMaster;
using TravoRides.Application.DTOs.Common;

namespace TravoRides.Application.Interfaces
{
    public interface IFeaturesMasterService
    {
        Task<PagedResponse<FeaturesMasterDTO>> GetAllAsync(SearchFeatureMasterRequest request, CancellationToken cancellationToken = default);
        Task<FeaturesMasterDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> CreateAsync(CreateFeaturesMasterRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(UpdateFeaturesMasterRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
