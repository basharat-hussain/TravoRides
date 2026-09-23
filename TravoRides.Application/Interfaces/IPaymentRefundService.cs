using TravoRides.Application.DTOs.Payment;

namespace TravoRides.Application.Interfaces
{
    public interface IPaymentRefundService
    {
        Task<RefundPaymentResponse> RefundPaymentAsync(Guid paymentId, RefundPaymentRequest request, string idempotencyKey, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RefundPaymentResponse>> GetRefundsAsync(Guid paymentId, CancellationToken cancellationToken = default);
    }
}
