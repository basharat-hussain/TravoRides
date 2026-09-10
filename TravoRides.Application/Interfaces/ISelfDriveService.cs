using TravoRides.Application.DTOs.SelfDrive;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Cabs;

namespace TravoRides.Application.Interfaces
{
    public interface ISelfDriveService
    {
        Task<PagedResponse<CabDTO>> GetAllAsync(SearchSelfDriveRequest request, CancellationToken cancellationToken = default);

        Task<SelfDriveDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<CabDTO?> GetByCabIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> CreateAsync(CreateSelfDriveRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(UpdateSelfDriveRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
