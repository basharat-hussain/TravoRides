using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Repositories
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<RefreshToken> GetByUserIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
