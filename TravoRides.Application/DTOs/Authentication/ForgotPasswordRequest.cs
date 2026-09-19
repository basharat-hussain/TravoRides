using System.ComponentModel.DataAnnotations;

namespace TravoRides.Application.DTOs.Authentication
{
    public class ForgotPasswordRequest
    {
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
