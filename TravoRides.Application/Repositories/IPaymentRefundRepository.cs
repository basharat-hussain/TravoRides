using TravoRides.Domain.Entities;

namespace TravoRides.Application.Repositories
{
    public interface IPaymentRefundRepository : IGenericRepository<PaymentRefund>
    {
        Task<IReadOnlyList<PaymentRefund>> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
        Task<PaymentRefund?> GetByGatewayRefundIdAsync(string gatewayRefundId, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalRefundedAmountAsync(Guid paymentId, CancellationToken cancellationToken = default);
        Task<PaymentRefund?> GetPendingByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PaymentRefund>> GetPendingRefundsAsync(int batchSize, CancellationToken cancellationToken = default);
        Task<PaymentRefund?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default);
        Task<Payment?> GetForRefundAsync(Guid paymentId, CancellationToken cancellationToken = default);
    }
}

