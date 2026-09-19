using System.ComponentModel.DataAnnotations;

namespace TravoRides.Application.DTOs.Authentication
{
    public class VerifyPasswordResetOtpRequest
    {
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string OTP { get; set; } = null!;
    }
}
