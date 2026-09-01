using TravoRides.Application.DTOs.SelfDrive;
using TravoRides.Application.DTOs.Common;

namespace TravoRides.Application.Interfaces
{
    public interface ISelfDriveService
    {
        Task<PagedResponse<SelfDriveDTO>> GetAllAsync(SearchSelfDriveRequest request, CancellationToken cancellationToken = default);
        Task<SelfDriveDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> CreateAsync(CreateSelfDriveRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(UpdateSelfDriveRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
