namespace TravoRides.Application.Interfaces.Services
{
    public interface IEmailTemplateService
    {
        public Task<string> GetEmailOTPVerificationTemplateAsync(string otp, int expiryMinutes);
        public Task<string> GetForgotPasswordOTPTemplateAsync(string otp, int expiryMinutes);
    }
}
