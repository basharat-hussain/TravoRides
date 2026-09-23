using System.Threading;
using System.Threading.Tasks;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Review;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Repositories
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<PagedResponse<Review>> GetAllSearchAsync(int pageNumber, int pageSize, string? keyword, CancellationToken cancellationToken);
        Task<PagedResponse<Review>> GetAllApprovedAsync(int pageNumber, int pageSize, string? keyword, CancellationToken cancellationToken);
    }
}

