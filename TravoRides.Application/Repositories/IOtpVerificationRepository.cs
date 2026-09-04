using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Domain.Enums;
using TravoRides.Application.Repositories;

namespace TravoRides.Application.Repositories
{
    public interface IOtpVerificationRepository : IGenericRepository<VerificationOtp>
    {
        Task<VerificationOtp?> GetActiveByUserIdAsync(Guid userId, VerificationOtpPurpose purpose, CancellationToken cancellationToken = default);
        Task InvalidateActiveOtpsAsync(Guid userId, VerificationOtpPurpose purpose, CancellationToken cancellationToken = default);
    }
}
