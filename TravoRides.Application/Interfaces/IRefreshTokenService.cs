using TravoRides.Application.DTOs.RefreshTokens;

namespace TravoRides.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<RefreshTokenDTO?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<RefreshTokenDTO?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Guid> CreateAsync(RefreshTokenDTO dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
