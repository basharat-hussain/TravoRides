using TravoRides.Domain.Entities;

namespace TravoRides.Application.Interfaces.Services
{
    public interface IPaymentStateService
    {
        Task MarkPaymentAsPaidAsync(Domain.Entities.Payment payment, Booking booking, string gatewayTransactionId, CancellationToken cancellationToken = default);
    }
}

