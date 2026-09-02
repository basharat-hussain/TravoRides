using TravoRiders.Application.DTOs.Authentication;
using TravoRiders.Application.Interfaces;
using TravoRiders.Application.Common.Exceptions;
using TravoRiders.Application.Interfaces.Services;
using TravoRiders.Domain.Entities;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;

namespace TravoRides.Application.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

            if (user == null)
            {
                throw new AuthenticationException("Invalid email or password.");
            }

            if (!user.IsActive)
            {
                throw new AuthenticationException("Your account has been deactivated.");
            }
            if (!user.IsEmailVerified)
            {
                throw new AuthenticationException("Please verify your email before logging in.");
            }

            var passwordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);

            if (!passwordValid)
            {
                throw new AuthenticationException("Invalid email or password.");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);

            var refreshTokenValue = _tokenService.GenerateRefreshToken();

            var accessTokenExpiresAt = _tokenService.GetAccessTokenExpiration();

            var refreshTokenExpiresAt = _tokenService.GetRefreshTokenExpiration();

            var refreshToken = new RefreshToken { UserId = user.Id, Token = refreshTokenValue, ExpiresAt = refreshTokenExpiresAt };

            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new LoginResponse
            {
                UserId = user.Id,
                Email = user.Email,
                Role = "Admin",

                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,

                AccessTokenExpiresAt = accessTokenExpiresAt,
                RefreshTokenExpiresAt = refreshTokenExpiresAt
            };
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                throw new AuthenticationException("Refresh token is required.");
            }

            var existingToken =
                await _refreshTokenRepository.GetByTokenAsync(
                    request.RefreshToken,
                    cancellationToken);

            if (existingToken == null)
            {
                throw new AuthenticationException("Invalid refresh token.");
            }

            if (!existingToken.IsActive)
            {
                throw new AuthenticationException("Refresh token has expired or has been revoked.");
            }

            var user = existingToken.User;

            if (user == null)
            {
                throw new AuthenticationException("User associated with refresh token was not found.");
            }

            if (!user.IsActive)
            {
                throw new AuthenticationException("User account is inactive.");
            }


            // Generate new tokens
            var accessToken = _tokenService.GenerateAccessToken(user);

            var newRefreshTokenValue =
                _tokenService.GenerateRefreshToken();

            var accessTokenExpiresAt =
                _tokenService.GetAccessTokenExpiration();

            var refreshTokenExpiresAt =
                _tokenService.GetRefreshTokenExpiration();

            // Revoke old refresh token
            existingToken.RevokedAt = DateTime.Now;
            existingToken.ReplacedByToken = newRefreshTokenValue;

            _unitOfWork.RefreshTokens.Update(existingToken);

            // Create new refresh token
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshTokenValue,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = refreshTokenExpiresAt
            };

            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

            _refreshTokenRepository.Update(existingToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return new RefreshTokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshTokenValue,
                AccessTokenExpiresAt = accessTokenExpiresAt,
                RefreshTokenExpiresAt = refreshTokenExpiresAt
            };
        }

        public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ValidationException("Refresh token is required.");
            }

            var existingToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);

            if (existingToken == null)
            {
                return;
            }

            if (!existingToken.RevokedAt.HasValue)
            {
                existingToken.RevokedAt = DateTime.UtcNow;

                _refreshTokenRepository.Update(existingToken);

                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);
            }
        }

        
    }
}