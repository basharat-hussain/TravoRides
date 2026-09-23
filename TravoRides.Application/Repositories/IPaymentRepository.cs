using TravoRides.Domain.Entities;

namespace TravoRides.Application.Repositories
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<Payment?> GetByPaymentNumberAsync(string paymentNumber, CancellationToken cancellationToken = default);
        Task<Payment?> GetByGatewayOrderIdAsync(string gatewayOrderId, CancellationToken cancellationToken = default);
        Task<Payment?> GetByGatewayTransactionIdAsync(string gatewayTransactionId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Payment>> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
        Task<Payment?> GetPendingByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
        Task<Payment?> GetSuccessfulPaymentAsync(Guid bookingId, Guid excludePaymentId, CancellationToken cancellationToken = default);
        Task<int> GetNextAttemptNumberAsync(Guid bookingId, CancellationToken cancellationToken = default);
    }
}

