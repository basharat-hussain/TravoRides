namespace TravoRides.Application.DTOs.Authentication
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;

        public DateTime AccessTokenExpiresAt { get; set; }

        public DateTime RefreshTokenExpiresAt { get; set; }

        public Guid UserId { get; set; }

        public string Email { get; set; } = null!;

        public string Role { get; set; } = null!;
    }
}
