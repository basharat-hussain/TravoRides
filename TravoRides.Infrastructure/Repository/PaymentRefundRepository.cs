using Microsoft.EntityFrameworkCore;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Domain.Enums;
using TravoRides.Infrastructure.Context;

namespace TravoRides.Infrastructure.Repository
{
    public class PaymentRefundRepository : GenericRepository<PaymentRefund>, IPaymentRefundRepository
    {
        private readonly ApplicationDbContext context;

        public PaymentRefundRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<IReadOnlyList<PaymentRefund>> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            return await context.PaymentRefunds
                .Where(x => x.PaymentId == paymentId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<PaymentRefund?> GetByGatewayRefundIdAsync(string gatewayRefundId, CancellationToken cancellationToken = default)
        {
            return await context.PaymentRefunds
                .FirstOrDefaultAsync(x => x.GatewayRefundId == gatewayRefundId, cancellationToken);
        }

        public async Task<decimal> GetTotalRefundedAmountAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            return await context.PaymentRefunds
                .Where(x => x.PaymentId == paymentId && x.Status == PaymentRefundStatus.Processed)
                .SumAsync(x => x.Amount, cancellationToken);
        }

        public async Task<PaymentRefund?> GetPendingByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            return await context.PaymentRefunds
                .FirstOrDefaultAsync(x => x.PaymentId == paymentId && x.Status == PaymentRefundStatus.Pending, cancellationToken);
        }

        public async Task<IReadOnlyList<PaymentRefund>> GetPendingRefundsAsync(int batchSize, CancellationToken cancellationToken = default)
        {
            return await context.PaymentRefunds
                .Where(x => x.Status == PaymentRefundStatus.Pending)
                .OrderBy(x => x.CreatedAt)
                .Take(batchSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<PaymentRefund?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
        {
            return await context.PaymentRefunds
                .FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey, cancellationToken);
        }

        public async Task<Payment?> GetForRefundAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            return await context.Payments
                .FromSqlInterpolated($@"SELECT * FROM Payments WITH (UPDLOCK, ROWLOCK) WHERE Id = {paymentId}")
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}

