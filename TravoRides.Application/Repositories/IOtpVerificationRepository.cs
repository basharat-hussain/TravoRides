using TravoRiders.Application.Repositories;
using TravoRiders.Domain.Entities;
using TravoRiders.Domain.Enums;
using TravoRides.Application.Repositories;

namespace TravoRiders.Application.Repositories
{
    public interface IOtpVerificationRepository : IGenericRepository<VerificationOtp>
    {
        Task<VerificationOtp?> GetActiveByUserIdAsync(Guid userId, VerificationOtpPurpose purpose, CancellationToken cancellationToken = default);
        Task InvalidateActiveOtpsAsync(Guid userId, VerificationOtpPurpose purpose, CancellationToken cancellationToken = default);
    }
}
