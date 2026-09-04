namespace TravoRides.Application.DTOs.Authentication
{
    public class VerifyPasswordResetOtpRequest
    {
        public string Email { get; set; } = null!;
        public string OTP { get; set; } = null!;
    }
}
