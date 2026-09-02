using TravoRiders.Application.DTOs.Users;

namespace TravoRides.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserOnlyResponse> RegisterUserAsync(CreateUserRequst request);
        Task<UserProfileResponse> GetMyProfileAsync(Guid userId);
    }
}
