using TravoRides.Domain.Entities;

namespace TravoRides.Application.Repositories
{
    public interface IPaymentWebhookRepository : IGenericRepository<PaymentWebhook>
    {
        Task<PaymentWebhook?> GetByEventIdAsync(string eventId, CancellationToken cancellationToken = default);
        Task<Payment?> GetByGatewayOrderIdAsync(string gatewayOrderId, CancellationToken cancellationToken = default);
        Task<PaymentWebhook?> GetForProcessingAsync(string eventId, CancellationToken cancellationToken = default);
    }
}

