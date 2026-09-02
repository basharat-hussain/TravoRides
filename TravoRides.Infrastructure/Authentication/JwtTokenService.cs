using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TravoRiders.Application.Interfaces;
using TravoRiders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TravoRides.Application.Interfaces;
using TravoRides.Domain.Entities;

namespace TravoRiders.Infrastructure.Authentication
{
    public class JwtTokenService : ITokenService
    {
        private readonly JwtSettings _settings;

        public JwtTokenService(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
        }
        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

                new(JwtRegisteredClaimNames.Email,user.Email),

                new(ClaimTypes.NameIdentifier,user.Id.ToString()),

                new(ClaimTypes.Email,user.Email),

               // new(ClaimTypes.Role,user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(randomBytes);
        }

        public DateTime GetAccessTokenExpiration()
        {
            return DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);
        }

        public DateTime GetRefreshTokenExpiration()
        {
            return DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);
        }

    }
}
