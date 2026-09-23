using TravoRides.Application.DTOs.Payment;

namespace TravoRides.Application.Interfaces.Services
{
    public interface IRazorPayPaymentWebhookService
    {
        Task ProcessWebhookAsync(RazorpayWebhookRequest request, string eventId, string signature, string rawPayload, CancellationToken cancellationToken = default);
        Task ReceiveWebhookAsync(string rawPayload, string signature, string eventId, CancellationToken cancellationToken = default);
    }
}

