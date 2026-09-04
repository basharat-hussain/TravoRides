using Microsoft.EntityFrameworkCore;
using TravoRides.Infrastructure.Repository;
using TravoRides.Domain.Entities;
using TravoRides.Application.Repositories;
using TravoRides.Infrastructure.Context;

namespace TravoRides.Infrastructure.Repository
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
        }

        public async Task<RefreshToken?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        }

        
    }
}
