using TravoRides.Application.DTOs.Users;

namespace TravoRides.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserOnlyResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<UserOnlyResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> CreateAsync(CreateUserRequst request, CancellationToken cancellationToken = default);
        Task UpdateAsync(UserOnlyResponse request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
