using TravoRiders.Domain.Enums;

namespace TravoRiders.Application.DTOs.Users
{
    public class CreateUserRequst
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
