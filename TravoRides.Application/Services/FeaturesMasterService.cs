using AutoMapper;
using TravoRides.Application.DTOs.FeaturesMaster;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Services
{
    public class FeaturesMasterService : IFeaturesMasterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeaturesMasterService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<FeaturesMasterDTO>> GetAllAsync(int pageNumber, int pageSize, string? keyword, CancellationToken cancellationToken = default)
        {
            var items = await _unitOfWork.FeatureMasters.GetAllAsync(cancellationToken);
            var dtos = _mapper.Map<IEnumerable<FeaturesMasterDTO>>(items);
            return new PagedResponse<FeaturesMasterDTO>
            {
                Items = dtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = dtos.Count(),
                TotalPages = 1
            };
        }

        public async Task<FeaturesMasterDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.FeatureMasters.GetByIdAsync(id, cancellationToken);
            if (entity == null) return null;
            return _mapper.Map<FeaturesMasterDTO>(entity);
        }

        public async Task<Guid> CreateAsync(CreateFeaturesMasterRequest request, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<FeaturesMaster>(request);
            await _unitOfWork.FeatureMasters.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }

        public async Task UpdateAsync(UpdateFeaturesMasterRequest request, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<FeaturesMaster>(request);
            _unitOfWork.FeatureMasters.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.FeatureMasters.GetByIdAsync(id, cancellationToken);
            if (entity == null) return;
            entity.IsDeleted = true;
            _unitOfWork.FeatureMasters.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
