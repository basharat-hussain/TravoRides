using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.Common;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<PagedResponse<Category>> GetAllSearchAsync(int pageNumber, int pageSize, string? keyword, CancellationToken cancellationToken);
    }
}
