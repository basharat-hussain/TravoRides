using Microsoft.EntityFrameworkCore;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Domain.Enums;
using TravoRides.Infrastructure.Context;
using TravoRides.Infrastructure.Repository;

namespace TravoRides.Infrastructure.Repository
{
    public class OtpVerificationRepository : GenericRepository<VerificationOtp>, IOtpVerificationRepository
    {
        private readonly ApplicationDbContext dbContext;

        public OtpVerificationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<VerificationOtp?> GetActiveByUserIdAsync(Guid userId, VerificationOtpPurpose purpose, CancellationToken cancellationToken = default)
        {
            return await dbContext.VerificationOtps
                .Where(x => x.UserId == userId && x.Purpose == purpose && !x.IsUsed && x.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task InvalidateActiveOtpsAsync(Guid userId, VerificationOtpPurpose purpose, CancellationToken cancellationToken = default)
        {
            var otps = await dbContext.VerificationOtps
                .Where(x => x.UserId == userId && x.Purpose == purpose && !x.IsUsed && x.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            foreach (var otp in otps)
            {
                otp.IsUsed = true;
                otp.UsedAt = DateTime.UtcNow;
            }
        }
    }
}
