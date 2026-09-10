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
    public class SelfDriveRepository : GenericRepository<SelfDrive>, ISelfDriveRepository
    {
        private readonly ApplicationDbContext context;
        public SelfDriveRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<PagedResponse<Cab>> GetAllSearchAsync(
            int pageNumber,
            int pageSize,
            string? keyword,
            Guid? cabId,
            CancellationToken cancellationToken)
        {
            var query = context.Cabs
                .Include(c => c.Category)
                .Include(c => c.CabFeatures)
                .Join(context.SelfDrives, c => c.Id, s => s.CabId, (c, s) => c)
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

            // Filter by cab
            if (cabId.HasValue)
            {
                query = query.Where(c => c.Id == cabId.Value);
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

        public async Task<Cab?> GetSelfDriveById(
      Guid id,
      CancellationToken cancellationToken)
        {
            var query = context.Cabs
                .AsNoTracking()
                .Include(c => c.Category)
                .Include(c => c.CabFeatures);

            return await query.FirstOrDefaultAsync(
                c => c.Id == id,
                cancellationToken);
        }
    }
}
