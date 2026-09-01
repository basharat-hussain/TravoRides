using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TravoRiders.Infrastructure.Context;
using TravoRides.Domain.Entities;
using TravoRides.Application.DTOs.Common;

namespace TravoRides.Infrastructure.Repository
{
    public class CategoryRepository : GenericRepository<Category>
    {
        private readonly ApplicationDbContext context;
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
        public async Task<PagedResponse<Category>> GetAllSearchAsync(int pageNumber, int pageSize, string? keyword, CancellationToken cancellationToken)
        {
            var query = context.Categories
                .Where(c => !c.IsDeleted)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var clean = keyword.Trim();
                query = query.Where(c => c.Name.Contains(clean) || (c.Description != null && c.Description.Contains(clean)));
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            return new PagedResponse<Category>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = totalPages
            };
        }
    }
}
