using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Domain.Common;
using TravoRiders.Domain.Entities;
using TravoRiders.Domain.Enums;

namespace TravoRides.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }

        public UserRole Role { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
