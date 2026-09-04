using TravoRides.Application.DTOs.Authentication;
using TravoRides.Domain.Enums;

namespace TravoRides.Application.Interfaces
{
    public interface IForgotPasswordService
    {
        Task SendForgotPasswordOtpAsync(ForgotPasswordRequest request, VerificationOtpPurpose purpose, CancellationToken cancellationToken = default);
        Task VerifyResetPasswordOtpAsync(VerifyPasswordResetOtpRequest request, CancellationToken cancellationToken = default);
        Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken);
    }
}
