namespace TravoRides.Application.Interfaces.Services
{
    public interface IEmailTemplateService
    {
        public Task<string> GetEmailOTPVerificationTemplateAsync(string otp, int expiryMinutes);
        public Task<string> GetForgotPasswordOTPTemplateAsync(string otp, int expiryMinutes);

        public Task<string> GetBookingConfirmationTemplateAsync(string name, string bookingId, DateTime bookingDate, string cabName, string cabType, decimal totalAmount);
        public Task<string> GetBookingCancellationTemplateAsync(string name, string bookingId, DateTime bookingDate, string cabName, string cabType, decimal totalAmount);
        public Task<string> GetBookingCompletionTemplateAsync(string name, string bookingId, DateTime bookingDate, string cabName, string cabType, decimal totalAmount);
        public Task<string> GetEnquiryConfirmationTemplateAsync(string name, string subject, string message, string phone);
    }
}
