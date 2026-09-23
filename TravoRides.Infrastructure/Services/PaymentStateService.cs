using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Domain.Enums;

namespace TravoRides.Infrastructure.Services
{
    public class PaymentStateService : IPaymentStateService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentStateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task MarkPaymentAsPaidAsync(Payment payment, Booking booking, string gatewayTransactionId, CancellationToken cancellationToken = default)
        {
            if (payment.Status == PaymentStatus.Paid)
                return Task.CompletedTask;

            payment.Status = PaymentStatus.Paid;
            payment.GatewayTransactionId = gatewayTransactionId;
            payment.PaidAt = DateTime.UtcNow;
            payment.ModifiedAt = DateTime.UtcNow;

            booking.IsConfirmed = true;
            booking.ModifiedAt = DateTime.UtcNow;

            _unitOfWork.Payments.Update(payment);
            _unitOfWork.Bookings.Update(booking);

            return Task.CompletedTask;
        }
    }
}

