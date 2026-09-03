using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TravoRiders.Application.DTOs.Review;
using TravoRiders.Domain.Entities;
using TravoRiders.Infrastructure.Context;
using TravoRides.Application.DTOs.Common;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Repository
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<PagedResponse<Review>> GetAllSearchAsync(int pageNumber, int pageSize, string? keyword, CancellationToken cancellationToken)
        {
            var query = _context.Reviews
                .Where(r => !r.IsDeleted)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var clean = keyword.Trim();
                query = query.Where(c => c.Name.Contains(clean) || (c.Address != null && c.Address.Contains(clean)));
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            return new PagedResponse<Review>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = totalPages
            };
        }
        public async Task<IEnumerable<Review>> GetAllApprovedAsync(CancellationToken cancellationToken = default)
        {
            // Public: Filters out inactive reviews directly in the SQL database
            return await _context.Reviews
                .Where(r => r.IsActive)
                .ToListAsync(cancellationToken);

        }
    }
}
