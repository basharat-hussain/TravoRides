using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Infrastructure.Context;

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
                .Where(c => !c.IsDeleted)
                .Join(
                    context.SelfDrives,
                    c => c.Id,
                    s => s.CabId,
                    (c, s) => new
                    {
                        Cab = c,
                        SelfDrive = s
                    })
                .AsNoTracking();

            // Search by keyword
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string cleanKeyword = keyword.Trim();

                query = query.Where(x =>
                    x.Cab.Name.Contains(cleanKeyword) ||
                    (x.Cab.Description != null &&
                     x.Cab.Description.Contains(cleanKeyword)));
            }

            // Filter by cab
            if (cabId.HasValue)
            {
                query = query.Where(x => x.Cab.Id == cabId.Value);
            }

            // Total records
            var totalCount = await query.CountAsync(cancellationToken);

            // Pagination + projection
            var items = await query
                .OrderByDescending(x => x.Cab.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new Cab
                {
                    Id = x.Cab.Id,
                    Name = x.Cab.Name,
                    Description = x.Cab.Description,

                    CategoryId = x.Cab.CategoryId,
                    Category = x.Cab.Category,

                    CabFeatures = x.Cab.CabFeatures,

                    LuggageCapacity = x.Cab.LuggageCapacity,
                    SeatingCapacity = x.Cab.SeatingCapacity,
                    Fuel = x.Cab.Fuel,
                    Transmission = x.Cab.Transmission,

                    // SelfDrive values
                    PricePerDay = x.SelfDrive.PricePerDay,
                    Discount = x.SelfDrive.Discount,

                    CreatedAt = x.Cab.CreatedAt
                })
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

        public async Task<Cab?> GetSelfDriveByCabId(Guid id, CancellationToken cancellationToken)
        {
            var query = context.Cabs
                .AsNoTracking()
                .Include(c => c.Category)
                .Include(c => c.CabFeatures)
            .Join(context.SelfDrives, c => c.Id, s => s.CabId, (c, s) => new Cab
            {
                Name = c.Name,
                Category = c.Category,
                CabFeatures = c.CabFeatures,
                LuggageCapacity = c.LuggageCapacity,
                SeatingCapacity = c.SeatingCapacity,
                Fuel = c.Fuel,
                Transmission = c.Transmission,
                PricePerDay = s.PricePerDay,
                Discount = s.Discount
            });
            return await query.FirstOrDefaultAsync(
                c => c.Id == id,
                cancellationToken);
        }

        public async Task<List<CabDTO>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            return await context.Cabs
                .Where(x => x.CategoryId == categoryId && !x.IsDeleted)
                .Select(x => new CabDTO
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }
        // for booking
        public async Task<SelfDrive?> GetByCabIdAsync(Guid cabId, CancellationToken cancellationToken)
        {
            return await context.SelfDrives
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.CabId == cabId &&
                         !x.IsDeleted,
                    cancellationToken);
        }
    }
}
