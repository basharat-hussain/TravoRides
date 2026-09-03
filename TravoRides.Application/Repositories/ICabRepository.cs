using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.Common;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Repositories
{
    public interface ICabRepository : IGenericRepository<Cab>
    {
        Task<PagedResponse<Cab>> GetAllSearchAsync(int pageNumber, int pageSize, string? keyword, Guid? categoryId, CancellationToken cancellationToken);
        Task<Cab?> GetCabByCategoryIdAsync(Guid id, CancellationToken cancellationToken);

    }
}
