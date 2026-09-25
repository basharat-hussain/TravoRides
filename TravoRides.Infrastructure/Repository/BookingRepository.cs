using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.BookingDTO;
using TravoRides.Application.DTOs.BookingReport;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Domain.Enums;
using TravoRides.Infrastructure.Context;

namespace TravoRides.Infrastructure.Repository
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext context;
        public BookingRepository(ApplicationDbContext context) :base(context)
        {
            this.context = context;
        }

        public async Task<PagedResponse<Booking>> GetAllSearchAsync(SearchBookingRequest request, CancellationToken cancellationToken = default)
        {
            var query = context.Bookings    
                .Where(b => !b.IsDeleted)
                .Include(b => b.Payments)
                .Include(b => b.Cab)
                .AsNoTracking()
                .AsQueryable();

            // Search by keyword across name, email, phone, and booking number
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                string cleanKeyword = request.Keyword.Trim();

                query = query.Where(c =>
                    c.Name.Contains(cleanKeyword) ||
                    (c.Email != null && c.Email.Contains(cleanKeyword)) ||
                    (c.Phone != null && c.Phone.Contains(cleanKeyword)) ||
                    (c.BookingNo != null && c.BookingNo.Contains(cleanKeyword)));
            }

            // Filter by confirmation status
            if (request.IsConfirmed.HasValue)
            {
                query = query.Where(c => c.IsConfirmed == request.IsConfirmed.Value);
            }

            var today = DateTime.Today;
            var scope = (request.BookingScope ?? "new").Trim().ToLowerInvariant();
            var quick = (request.QuickFilter ?? "").Trim().ToLowerInvariant().Replace("-", "").Replace("_", "");

            DateTime? computedFrom = request.FromDate?.Date;
            DateTime? computedTo = request.ToDate?.Date;

            if (!computedFrom.HasValue && !computedTo.HasValue && !string.IsNullOrWhiteSpace(quick) && quick != "all")
            {
                if (scope == "past")
                {
                    switch (quick)
                    {
                        case "yesterday":
                            computedFrom = today.AddDays(-1);
                            computedTo = today.AddDays(-1);
                            break;
                        case "lastweek":
                            computedFrom = today.AddDays(-7);
                            computedTo = today.AddDays(-1);
                            break;
                        case "lastmonth":
                            computedFrom = today.AddDays(-30);
                            computedTo = today.AddDays(-1);
                            break;
                    }
                }
                else // new / upcoming
                {
                    switch (quick)
                    {
                        case "today":
                            computedFrom = today;
                            computedTo = today;
                            break;
                        case "nextday":
                        case "tomorrow":
                            computedFrom = today.AddDays(1);
                            computedTo = today.AddDays(1);
                            break;
                        case "thisweek":
                            int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)today.DayOfWeek + 7) % 7;
                            computedFrom = today;
                            computedTo = today.AddDays(daysUntilSunday);
                            break;
                        case "thismonth":
                            computedFrom = today;
                            computedTo = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
                            break;
                    }
                }
            }

            if (computedFrom.HasValue)
            {
                query = query.Where(b => b.TravelDate >= computedFrom.Value);
            }

            if (computedTo.HasValue)
            {
                var endDate = computedTo.Value.AddDays(1);
                query = query.Where(b => b.TravelDate < endDate);
            }

            // If no specific date filter or quick filter was given, enforce scope boundary
            if (!computedFrom.HasValue && !computedTo.HasValue)
            {
                if (scope == "past")
                {
                    query = query.Where(b => b.TravelDate < today);
                }
                else if (scope != "all")
                {
                    // Default: new/upcoming bookings
                    query = query.Where(b => b.TravelDate >= today);
                }
            }

            // Total records
            var totalCount = await query.CountAsync(cancellationToken);

            // Sorting:
            // "by default new bookings should be displayed starting from today and then i.e. ascending date"
            if (scope == "past")
            {
                query = query
                    .OrderByDescending(c => c.TravelDate)
                    .ThenByDescending(c => c.PickupTime);
            }
            else
            {
                query = query
                    .OrderBy(c => c.TravelDate)
                    .ThenBy(c => c.PickupTime);
            }

            // Pagination
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            return new PagedResponse<Booking>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<BookingReportResponse> GetBookingReportAsync( int pageNumber, int pageSize, string? keyword,
                 DateTime? fromDate, DateTime? toDate,bool? isConfirmed, CancellationToken cancellationToken)
        {
            var query = context.Bookings.Where(b => !b.IsDeleted).AsNoTracking().AsQueryable();

            // -----------------------------
            // KEYWORD SEARCH
            // -----------------------------

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var cleanKeyword = keyword.Trim();

                query = query.Where(b =>
                    b.Name.Contains(cleanKeyword) ||
                    b.Email.Contains(cleanKeyword) ||
                    b.BookingNo.Contains(cleanKeyword));
            }
            // IsConfirmed Filter

            if (isConfirmed.HasValue)
            {
                query = query.Where(x =>
                    x.IsConfirmed == isConfirmed.Value);
            }
            // -----------------------------
            // BOOKING DATE FILTER
            // -----------------------------

            if (fromDate.HasValue)
            {
                query = query.Where(b =>
                    b.CreatedAt >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                var endDate = toDate.Value.Date.AddDays(1);

                query = query.Where(b =>
                    b.CreatedAt < endDate);
            }

            // -----------------------------
            // SUMMARY
            // -----------------------------

            var totalBookings = await query
                .CountAsync(cancellationToken);

            var confirmedBookings = await query
                .CountAsync(
                    b => b.IsConfirmed,
                    cancellationToken);

            var pendingBookings = await query
                .CountAsync(
                    b => !b.IsConfirmed,
                    cancellationToken);

            var totalAmount = await query
                .SelectMany(b => b.Payments)
                .Where(p =>
                    p.Status == PaymentStatus.Paid)
                .Select(p => (decimal?)p.Amount)
                .SumAsync(cancellationToken) ?? 0;

            var successfulAmount = totalAmount;

            var failedAmount = await query
                .SelectMany(b => b.Payments)
                .Where(p =>
                    p.Status == PaymentStatus.Failed)
                .Select(p => (decimal?)p.Amount)
                .SumAsync(cancellationToken) ?? 0;

            var summary = new BookingSummaryDTO
            {
                TotalBookings = totalBookings,
                ConfirmedBookings = confirmedBookings,
                PendingBookings = pendingBookings,

                TotalAmount = totalAmount,
                SuccessfulAmount = successfulAmount,
                FailedAmount = failedAmount
            };

            // -----------------------------
            // PAGINATION
            // -----------------------------

            var totalPages = (int)Math.Ceiling(
                (double)totalBookings / pageSize);

            var items = await query
                .OrderByDescending(b => b.CreatedAt)

                .Skip((pageNumber - 1) * pageSize)

                .Take(pageSize)

                .Select(b => new BookingReportDTO
                {
                    Id = b.Id,

                    BookingNo = b.BookingNo,

                    Name = b.Name,

                    Email = b.Email,

                    Phone = b.Phone,

                    BookingDate = b.CreatedAt,

                    TravelDate = b.TravelDate,
                    ReturnDate = b.ReturnDate,

                    PickupLocation = b.PickupLocation,

                    DropLocation = b.DropLocation,

                    IsConfirmed = b.IsConfirmed,

                    // Number of payment attempts
                    PaymentAttempts = b.Payments.Count(),

                    // Get successful payment
                    PaymentAmount = b.Payments
                        .Where(p =>
                            p.Status == PaymentStatus.Paid)
                        .OrderByDescending(p => p.AttemptNumber)
                        .Select(p => (decimal?)p.Amount)
                        .FirstOrDefault(),

                    PaymentStatus = b.Payments
                        .Where(p =>
                            p.Status == PaymentStatus.Paid)
                        .OrderByDescending(p => p.AttemptNumber)
                        .Select(p => (PaymentStatus?)p.Status)
                        .FirstOrDefault(),

                    PaidAt = b.Payments
                        .Where(p =>
                            p.Status == PaymentStatus.Paid)
                        .OrderByDescending(p => p.AttemptNumber)
                        .Select(p => p.PaidAt)
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            // -----------------------------
            // FINAL RESPONSE
            // -----------------------------

            return new BookingReportResponse
            {
                Summary = summary,

                Bookings = new PagedResponse<BookingReportDTO>
                {
                    Items = items,

                    PageNumber = pageNumber,

                    PageSize = pageSize,

                    TotalCount = totalBookings,

                    TotalPages = totalPages
                }
            };
        }

        public new async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Bookings
                .Include(x => x.Cab)
                .Include(x => x.Transit)
                .Include(x => x.Package)
                .Include(x => x.Payments)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id &&
                         !x.IsDeleted,
                    cancellationToken);
        }
    }
}
