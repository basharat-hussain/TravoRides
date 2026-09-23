using TravoRides.Application.Interfaces.Notifications;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.Repositories;

namespace TravoRides.Application.Services
{
    public class BookingNotificationService : IBookingNotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;

        public BookingNotificationService(IUnitOfWork unitOfWork, IEmailService emailService, IEmailTemplateService emailTemplateService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
        }

        public async Task SendBookingConfirmationAsync(Guid bookingId, CancellationToken cancellationToken = default)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId, cancellationToken);
            if (booking == null) return;

            var cab = await _unitOfWork.Cabs.GetByIdAsync(booking.CabId, cancellationToken);
            var cabName = cab?.Name ?? "Standard Cab";
            var cabType = booking.BookingType.ToString();

            var emailBody = await _emailTemplateService.GetBookingConfirmationTemplateAsync(
                booking.Name,
                string.IsNullOrWhiteSpace(booking.BookingNo) ? booking.Id.ToString() : booking.BookingNo,
                booking.TravelDate,
                cabName,
                cabType,
                booking.TotalAmount);

            if (!string.IsNullOrWhiteSpace(booking.Email))
            {
                await _emailService.SendEmailAsync(
                    booking.Email,
                    $"TravoRides - Booking Confirmed #{booking.BookingNo}",
                    emailBody,
                    withHeaderLogo: true,
                    cancellationToken);
            }
        }
    }
}

