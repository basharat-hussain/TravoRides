using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.Common;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Repositories
{
    public interface ISelfDriveRepository : IGenericRepository<SelfDrive>
    {
        Task<PagedResponse<Cab>> GetAllSearchAsync(int pageNumber, int pageSize, string? keyword, Guid? cabId, CancellationToken cancellationToken);
        Task<Cab?> GetSelfDriveById(Guid id, CancellationToken cancellationToken);
    }
}
