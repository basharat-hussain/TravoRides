using TravoRides.Domain.Enums;

namespace TravoRides.Application.Interfaces
{
    public interface IOtpVerificationService
    {
        Task SendOtpAsync(string email, VerificationOtpPurpose purpose, CancellationToken cancellationToken = default);
        Task<bool> VerifyOtpAsync(string email, string otp, VerificationOtpPurpose purpose, CancellationToken cancellationToken = default);
    }
}
