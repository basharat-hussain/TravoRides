namespace TravoRides.Application.DTOs.Users
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsEmailVerified { get; set; }
    }
}
