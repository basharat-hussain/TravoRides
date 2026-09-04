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
    public class FeatureMasterRepository : GenericRepository<FeaturesMaster>, IFeatureMasterRepository
    {
        private readonly ApplicationDbContext context;
        public FeatureMasterRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<PagedResponse<FeaturesMaster>> GetAllSearchAsync(
       int pageNumber,
       int pageSize,
       string? keyword,

       CancellationToken cancellationToken)
        {
            var query = context.FeatureMasters

                .Where(c => !c.IsDeleted)
                .AsNoTracking()
                .AsQueryable();

            // Search by keyword
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string cleanKeyword = keyword.Trim();

                query = query.Where(c =>
                    c.Title.Contains(cleanKeyword) ||
                    (c.Description != null &&
                     c.Description.Contains(cleanKeyword)));
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

            return new PagedResponse<FeaturesMaster>
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
