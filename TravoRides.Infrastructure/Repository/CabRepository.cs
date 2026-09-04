using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Infrastructure.Context;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Repository
{
    public class CabRepository : GenericRepository<Cab>, ICabRepository
    {
        private readonly ApplicationDbContext context;
        public CabRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<PagedResponse<Cab>> GetAllSearchAsync(
            int pageNumber,
            int pageSize,
            string? keyword,
            Guid? categoryId,
            CancellationToken cancellationToken)
        {
            var query = context.Cabs
                .Include(c => c.Category)
                .Where(c => !c.IsDeleted)
                .AsNoTracking()
                .AsQueryable();

            // Search by keyword
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string cleanKeyword = keyword.Trim();

                query = query.Where(c =>
                    c.Name.Contains(cleanKeyword) ||
                    (c.Description != null &&
                     c.Description.Contains(cleanKeyword)));
            }

            // Filter by category
            if (categoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == categoryId.Value);
            }

            // Total records
            var totalCount = await query.CountAsync(cancellationToken);

            // Pagination
            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling(
                (double)totalCount / pageSize);

            return new PagedResponse<Cab>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
      

        public async Task<Cab?> GetCabByCategoryIdAsync(Guid cabId, CancellationToken cancellationToken)
        {
          

            return await context.Cabs
        .Include(c => c.Category)
        .AsNoTracking()
        .FirstOrDefaultAsync(
            c => c.Id == cabId && !c.IsDeleted,
            cancellationToken);
        }
    }

}
