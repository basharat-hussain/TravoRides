using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Infrastructure.Context;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Repository
{
    public class SelfDriveRepository : GenericRepository<SelfDrive>, ISelfDriveRepository
    {
        private readonly ApplicationDbContext context;
        public SelfDriveRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<PagedResponse<SelfDrive>> GetAllSearchAsync(
            int pageNumber,
            int pageSize,
            string? keyword,
            Guid? cabId,
            CancellationToken cancellationToken)
        {
            var query = context.SelfDrives
                .Include(c => c.Cab)
                .Where(c => !c.IsDeleted)
                .AsNoTracking()
                .AsQueryable();

            // Search by keyword
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string cleanKeyword = keyword.Trim();

                query = query.Where(c =>
                    c.Cab.Name.Contains(cleanKeyword) ||
                    (c.Cab.Description != null &&
                     c.Cab.Description.Contains(cleanKeyword)));
            }

            // Filter by cab
            if (cabId.HasValue)
            {
                query = query.Where(c => c.CabId == cabId.Value);
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

            return new PagedResponse<SelfDrive>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
        public async Task<SelfDrive?> GetSelfDriveByCabAsync(Guid selfDriveId, CancellationToken cancellationToken)
        {
            return await context.SelfDrives
         .Include(c => c.Cab)
         .AsNoTracking()
         .FirstOrDefaultAsync(
             c => c.Id == selfDriveId && !c.IsDeleted,
             cancellationToken);
        }
    }
}
