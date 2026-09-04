using TravoRides.Application.DTOs.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

       Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);

        Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
