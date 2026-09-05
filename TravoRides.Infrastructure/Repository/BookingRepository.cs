using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
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

        public async Task<BookingReportResponse> GetBookingReportAsync( int pageNumber, int pageSize, string? keyword,
                 DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken)
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
                    p.PaymentStatus == PaymentStatus.Paid)
                .Select(p => (decimal?)p.Amount)
                .SumAsync(cancellationToken) ?? 0;

            var successfulAmount = totalAmount;

            var failedAmount = await query
                .SelectMany(b => b.Payments)
                .Where(p =>
                    p.PaymentStatus == PaymentStatus.Failed)
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

                    PickupLocation = b.PickupLocation,

                    DropLocation = b.DropLocation,

                    IsConfirmed = b.IsConfirmed,

                    // Number of payment attempts
                    PaymentAttempts = b.Payments.Count(),

                    // Get successful payment
                    PaymentAmount = b.Payments
                        .Where(p =>
                            p.PaymentStatus == PaymentStatus.Paid)
                        .OrderByDescending(p => p.AttemptNumber)
                        .Select(p => (decimal?)p.Amount)
                        .FirstOrDefault(),

                    PaymentStatus = b.Payments
                        .Where(p =>
                            p.PaymentStatus == PaymentStatus.Paid)
                        .OrderByDescending(p => p.AttemptNumber)
                        .Select(p => (PaymentStatus?)p.PaymentStatus)
                        .FirstOrDefault(),

                    PaidAt = b.Payments
                        .Where(p =>
                            p.PaymentStatus == PaymentStatus.Paid)
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
    }
}
