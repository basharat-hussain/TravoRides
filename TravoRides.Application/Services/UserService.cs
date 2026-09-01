using AutoMapper;
using TravoRides.Application.DTOs.Users;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserOnlyResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var items = await _unitOfWork.Users.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<UserOnlyResponse>>(items);
        }

        public async Task<UserOnlyResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
            if (entity == null) return null;
            return _mapper.Map<UserOnlyResponse>(entity);
        }

        public async Task<Guid> CreateAsync(CreateUserRequst request, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<User>(request);
            await _unitOfWork.Users.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }

        public async Task UpdateAsync(UserOnlyResponse request, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<User>(request);
            _unitOfWork.Users.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
            if (entity == null) return;
            entity.IsDeleted = true;
            _unitOfWork.Users.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
