using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Infrastructure.Repository
{
    public class LatestThinkingRepository : GenericRepository<LatestThinking>, ILatestThinkingRepository
    {
        private readonly ApplicationDbContext _context;
        public LatestThinkingRepository(ApplicationDbContext context) : base(context)
        {
            context = context;
        }

        public async Task<PagedResponse<LatestThinking>> GetAllSearchAsync(int pageNumber, int pageSize, string? keyword, string? author, CancellationToken cancellationToken)
        {
            var query = _context.Set<LatestThinking>().Where(p => !p.IsDeleted).AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var clean = keyword.Trim().ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(clean)
                                      || (p.Description != null && p.Description.ToLower().Contains(clean)));
            }

            if (!string.IsNullOrWhiteSpace(author))
            {
                query = query.Where(p => p.Author == author.Trim());
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(p => p.PublishedOn ?? p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new PagedResponse<LatestThinking>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}
