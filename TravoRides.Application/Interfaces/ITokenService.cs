using TravoRides.Domain.Entities;

namespace TravoRides.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);

        string GenerateRefreshToken();

        DateTime GetAccessTokenExpiration();

        DateTime GetRefreshTokenExpiration();
    }
}
