using Microsoft.EntityFrameworkCore;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Domain.Enums;
using TravoRides.Infrastructure.Context;

namespace TravoRides.Infrastructure.Repository
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Payment?> GetByPaymentNumberAsync(string paymentNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(x => x.PaymentNumber == paymentNumber, cancellationToken);
        }

        public async Task<Payment?> GetByGatewayOrderIdAsync(string gatewayOrderId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(x => x.GatewayOrderId == gatewayOrderId, cancellationToken);
        }

        public async Task<Payment?> GetByGatewayTransactionIdAsync(string gatewayTransactionId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(x => x.GatewayTransactionId == gatewayTransactionId, cancellationToken);
        }

        public async Task<IReadOnlyList<Payment>> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(x => x.BookingId == bookingId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Payment?> GetPendingByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(x => x.BookingId == bookingId && x.Status == PaymentStatus.Pending, cancellationToken);
        }

        public async Task<Payment?> GetSuccessfulPaymentAsync(Guid bookingId, Guid excludePaymentId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(x => x.BookingId == bookingId && x.Id != excludePaymentId && x.Status == PaymentStatus.Paid, cancellationToken);
        }

        public async Task<int> GetNextAttemptNumberAsync(Guid bookingId, CancellationToken cancellationToken = default)
        {
            var lastAttemptNumber = await _context.Payments
                .Where(x => x.BookingId == bookingId)
                .Select(x => (int?)x.AttemptNumber)
                .MaxAsync(cancellationToken) ?? 0;

            return lastAttemptNumber + 1;
        }

        public async Task<TravoRides.Application.DTOs.Common.PagedResponse<Payment>> GetAllSearchAsync(
            int pageNumber,
            int pageSize,
            string? keyword,
            PaymentStatus? status,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Payments
                .Where(p => !p.IsDeleted)
                .Include(p => p.Booking)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var clean = keyword.Trim();
                query = query.Where(p =>
                    p.PaymentNumber.Contains(clean) ||
                    (p.GatewayTransactionId != null && p.GatewayTransactionId.Contains(clean)) ||
                    (p.GatewayOrderId != null && p.GatewayOrderId.Contains(clean)) ||
                    (p.Booking != null && (
                        p.Booking.BookingNo.Contains(clean) ||
                        p.Booking.Name.Contains(clean) ||
                        (p.Booking.Email != null && p.Booking.Email.Contains(clean)) ||
                        (p.Booking.Phone != null && p.Booking.Phone.Contains(clean))
                    )));
            }

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(p => (p.PaidAt ?? p.CreatedAt) >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                var endDate = toDate.Value.Date.AddDays(1);
                query = query.Where(p => (p.PaidAt ?? p.CreatedAt) < endDate);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(p => p.PaidAt ?? p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new TravoRides.Application.DTOs.Common.PagedResponse<Payment>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<Payment?> GetByIdWithBookingAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .Include(p => p.Booking)
                .Include(p => p.PaymentRefunds)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
        }
    }
}

