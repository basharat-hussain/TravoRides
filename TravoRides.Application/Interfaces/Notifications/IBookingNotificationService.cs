namespace TravoRides.Application.Interfaces.Notifications
{
    public interface IBookingNotificationService
    {
        Task SendBookingConfirmationAsync(Guid bookingId, CancellationToken cancellationToken = default);
    }
}

