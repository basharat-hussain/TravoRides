using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace AlArwaSolutions.Application.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);


        Task<User?> GetByEmailWithRefreshTokensAsync(string email, CancellationToken cancellationToken = default);

        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    }
}
