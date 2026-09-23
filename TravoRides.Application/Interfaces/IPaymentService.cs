using TravoRides.Application.DTOs.Payment;

namespace TravoRides.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<CreatePaymentResponse> CreatePaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default);
        Task<PaymentVerificationResponse> VerifyPaymentAsync(Guid paymentId, VerifyPaymentRequest request, CancellationToken cancellationToken = default);
    }
}
