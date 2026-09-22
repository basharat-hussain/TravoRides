using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.DTOs.Authentication;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Services
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
            if (request == null)
            {
                throw new ValidationException("Login request cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new ValidationException("Email is required.");
            }

            var user = new User();
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var email = request.Email.Trim().ToLowerInvariant();
                user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            }

            if (user == null)
            {
                throw new AuthenticationException($"Invalid email or password.");
            }

            if (!user.IsActive)
            {
                throw new AuthenticationException("Your account has been deactivated.");
            }



            var passwordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);

            if (!passwordValid)
            {
                throw new AuthenticationException($"Invalid email or password.");
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
                Role = user.Role.ToString(),
                
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
                ExpiresAt= refreshTokenExpiresAt
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
        public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
            {
                throw new ValidationException("User ID is required.");
            }

            if (request == null)
            {
                throw new ValidationException("Change password request cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            {
                throw new ValidationException("Current password is required.");
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                throw new ValidationException("New password is required.");
            }

            if (string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                throw new ValidationException("Confirm password is required.");
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                throw new ValidationException("New password and confirm password do not match.");
            }

            if (request.CurrentPassword == request.NewPassword)
            {
                throw new ValidationException("New password must be different from current password.");
            }

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

            if (user == null)
            {
                throw new ResourceNotFoundException("User not found.");
            }

            var isCurrentPasswordValid = _passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash);

            if (!isCurrentPasswordValid)
            {
                throw new AuthenticationException("Current password is incorrect.");
            }

            var hashedPassword = _passwordHasher.HashPassword(request.NewPassword);
            user.PasswordHash = hashedPassword;

            _userRepository.Update(user);

            // Invalidate all refresh tokens for this user
            var refreshTokens = await _refreshTokenRepository.GetAllByUserIdAsync(userId, cancellationToken);

            if (refreshTokens != null && refreshTokens.Count > 0)
            {
                foreach (var refreshToken in refreshTokens)
                {
                    if (!refreshToken.RevokedAt.HasValue)
                    {
                        refreshToken.RevokedAt = DateTime.UtcNow;
                        _refreshTokenRepository.Update(refreshToken);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }


    }
}