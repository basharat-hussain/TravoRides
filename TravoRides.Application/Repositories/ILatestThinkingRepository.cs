using TravoRides.Application.DTOs.Common;
using TravoRides.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.Repositories
{
    public interface ILatestThinkingRepository : IGenericRepository<LatestThinking>
    {
        Task<PagedResponse<LatestThinking>> GetAllSearchAsync(int pageNumber, int pageSize, string? keyword, string? author, CancellationToken cancellationToken);
    }
}
