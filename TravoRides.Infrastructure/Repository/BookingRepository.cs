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
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext context;
        public BookingRepository(ApplicationDbContext context) :base(context)
        {
            this.context = context;
        }

        public async Task<PagedResponse<Booking>> GetAllSearchAsync(
           int pageNumber,
           int pageSize,
           string? keyword,
           
           CancellationToken cancellationToken)
        {
            var query = context.Bookings    
                .Where(b => !b.IsDeleted)
                .AsNoTracking()
                .AsQueryable();

            // Search by keyword
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string cleanKeyword = keyword.Trim();

                query = query.Where(c =>
                    c.Name.Contains(cleanKeyword) ||
                    (c.Email != null &&
                     c.Email.Contains(cleanKeyword)));
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

            return new PagedResponse<Booking>
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
