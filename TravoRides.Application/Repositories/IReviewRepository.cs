using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TravoRiders.Application.DTOs.Review;
using TravoRiders.Domain.Entities;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Repository
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<PagedResponse<Review>> GetAllSearchAsync(int pageNumber, int pageSize, string? keyword, CancellationToken cancellationToken);
        Task<IEnumerable<Review>> GetAllApprovedAsync(CancellationToken cancellationToken = default);

    }
}
