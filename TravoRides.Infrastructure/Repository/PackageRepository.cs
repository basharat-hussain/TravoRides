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
    public class PackageRepository : GenericRepository<Package>, IPackageRepository
    {
        private readonly ApplicationDbContext context;
        public PackageRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
        public async Task<PagedResponse<Package>> GetAllAsync(
       int pageNumber,
       int pageSize,
       string? keyword,

       CancellationToken cancellationToken)
        {
            var query = context.Packages

                .Where(c => !c.IsDeleted)
                .AsNoTracking()
                .AsQueryable();

            // Search by keyword
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string cleanKeyword = keyword.Trim();

                query = query.Where(c =>
                    c.Title.Contains(cleanKeyword) ||
                    (c.Itinerary != null &&
                     c.Itinerary.Contains(cleanKeyword)));
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

            return new PagedResponse<Package>
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
