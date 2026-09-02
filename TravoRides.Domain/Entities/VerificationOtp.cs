
using TravoRiders.Domain.Common;
using TravoRiders.Domain.Enums;
using TravoRides.Domain.Entities;

namespace TravoRiders.Domain.Entities
{
    public class VerificationOtp : BaseEntity
    {
        public Guid UserId { get; set; }
        public string OTPHash { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; }

        public int AttemptCount { get; set; }
        public DateTime? UsedAt { get; set; }

        public VerificationOtpPurpose Purpose { get; set; }

        public User User { get; set; } = null!;



    }
}
