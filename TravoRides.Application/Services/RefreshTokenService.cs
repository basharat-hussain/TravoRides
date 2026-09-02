using AutoMapper;
using TravoRiders.Domain.Entities;
using TravoRides.Application.DTOs.RefreshTokens;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RefreshTokenService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<RefreshTokenDTO?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.RefreshTokens.GetByTokenAsync(token, cancellationToken);
            if (entity == null) return null;
            return _mapper.Map<RefreshTokenDTO>(entity);
        }

        public async Task<RefreshTokenDTO?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.RefreshTokens.GetByUserIdAsync(userId, cancellationToken);
            if (entity == null) return null;
            return _mapper.Map<RefreshTokenDTO>(entity);
        }

        public async Task<Guid> CreateAsync(RefreshTokenDTO dto, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<RefreshToken>(dto);
            await _unitOfWork.RefreshTokens.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.RefreshTokens.GetByIdAsync(id, cancellationToken);
            if (entity == null) return;
            entity.IsDeleted = true;
            _unitOfWork.RefreshTokens.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
