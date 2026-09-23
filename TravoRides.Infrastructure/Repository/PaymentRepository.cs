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
    }
}

