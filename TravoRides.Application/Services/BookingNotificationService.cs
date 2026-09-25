using Microsoft.Extensions.Options;
using TravoRides.Application.Common.Models;
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
        private readonly EmailSettings _emailSettings;

        public BookingNotificationService(
            IUnitOfWork unitOfWork, 
            IEmailService emailService, 
            IEmailTemplateService emailTemplateService,
            IOptions<EmailSettings> emailSettings)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _emailSettings = emailSettings.Value;
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

            var subject = $"TravoRides - Booking Confirmed #{booking.BookingNo}";
            var ownerEmail = _emailSettings.OwnerEmail?.Trim();

            if (!string.IsNullOrWhiteSpace(booking.Email))
            {
                var ccEmail = !string.IsNullOrWhiteSpace(ownerEmail) &&
                              !string.Equals(booking.Email, ownerEmail, StringComparison.OrdinalIgnoreCase)
                    ? ownerEmail
                    : null;

                await _emailService.SendEmailAsync(
                    booking.Email,
                    subject,
                    emailBody,
                    withHeaderLogo: true,
                    cc: ccEmail,
                    cancellationToken: cancellationToken);
            }
           
        }
    }
}

