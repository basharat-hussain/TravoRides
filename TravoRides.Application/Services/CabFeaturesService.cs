using AutoMapper;
using TravoRides.Application.DTOs.CabFeatures;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Services
{
    public class CabFeaturesService : ICabFeaturesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CabFeaturesService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<CabFeaturesDTO>> GetAllAsync(int pageNumber, int pageSize, string? keyword, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 8;

            // simple implementation using repository AsQueryable
            var items = await _unitOfWork.CabFeatures.GetAllAsync(cancellationToken);
            var dtos = _mapper.Map<IEnumerable<CabFeaturesDTO>>(items);

            return new PagedResponse<CabFeaturesDTO>
            {
                Items = dtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = dtos.Count(),
                TotalPages = 1
            };
        }

        public async Task<CabFeaturesDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.CabFeatures.GetByIdAsync(id, cancellationToken);
            if (entity == null) return null;
            return _mapper.Map<CabFeaturesDTO>(entity);
        }

        public async Task<Guid> CreateAsync(CreateCabFeaturesRequest request, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<CabFeatures>(request);
            await _unitOfWork.CabFeatures.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }

        public async Task UpdateAsync(UpdateCabFeaturesRequest request, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<CabFeatures>(request);
            _unitOfWork.CabFeatures.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.CabFeatures.GetByIdAsync(id, cancellationToken);
            if (entity == null) return;
            entity.IsDeleted = true;
            _unitOfWork.CabFeatures.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
