using TravoRides.Application.DTOs.Payment;

namespace TravoRides.Application.Interfaces.Payment
{
    public interface IPaymentGateway
    {
        string GatewayName { get; }
        string PublicKey { get; }
        Task<PaymentGatewayOrderResult> CreateOrderAsync(PaymentGatewayOrderRequest request, CancellationToken cancellationToken = default);
        Task<PaymentGatewayVerificationResult> VerifyPaymentAsync(PaymentGatewayVerificationRequest request, CancellationToken cancellationToken = default);
        Task<PaymentGatewayRefundResult> RefundAsync(PaymentGatewayRefundRequest request, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PaymentGatewayRefundResult>> GetRefundsAsync(string gatewayTransactionId, CancellationToken cancellationToken);
    }
}
