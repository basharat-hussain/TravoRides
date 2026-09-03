using AutoMapper;
using TravoRiders.Application.Common.Exceptions;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.FeaturesMaster;
using TravoRides.Application.DTOs.SelfDrive;
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

        public async Task<PagedResponse<FeaturesMasterDTO>> GetAllAsync(SearchFeatureMasterRequest request, CancellationToken cancellationToken = default)
        {
            // Defensive pagination
            if (request.PageNumber < 1)
                request.PageNumber = 1;

            if (request.PageSize < 1)
                request.PageSize = 8;

            if (request.PageSize > 100)
                request.PageSize = 100;

            var pagedResponse = await _unitOfWork.FeatureMasters
                .GetAllSearchAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.Keyword,
                    cancellationToken);

            var featuresMasterDtos = _mapper.Map<IEnumerable<FeaturesMasterDTO>>(
                pagedResponse.Items);


            return new PagedResponse<FeaturesMasterDTO>
            {
                Items = featuresMasterDtos,
                PageNumber = pagedResponse.PageNumber,
                PageSize = pagedResponse.PageSize,
                TotalCount = pagedResponse.TotalCount,
                TotalPages = pagedResponse.TotalPages
            };
        }

        public async Task<FeaturesMasterDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)                  
        {
            var featuresMaster = await _unitOfWork.FeatureMasters.GetByIdAsync(id, cancellationToken);
            if (featuresMaster == null) return null;
            return _mapper.Map<FeaturesMasterDTO>(featuresMaster);
        }

        public async Task<Guid> CreateAsync(CreateFeaturesMasterRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ValidationException("Title is required");

            var featuresMaster = new FeaturesMaster
            {
                Title = request.Title?.Trim(),
                Description = request.Description?.Trim(),
                Icon = request.Icon?.Trim() ?? string.Empty,
            };

            await _unitOfWork.FeatureMasters.AddAsync(featuresMaster, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return featuresMaster.Id;
        }

        public async Task UpdateAsync(UpdateFeaturesMasterRequest request, CancellationToken cancellationToken = default)
        {
            var featuresMaster = await _unitOfWork.FeatureMasters.GetByIdAsync(request.Id, cancellationToken);
            if (featuresMaster == null) throw new ResourceNotFoundException("FeaturesMaster not found.");

            featuresMaster.Title = request.Title?.Trim();
            featuresMaster.Description = request.Description?.Trim();
            featuresMaster.Icon = request.Icon?.Trim() ?? string.Empty;

            _unitOfWork.FeatureMasters.Update(featuresMaster);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var featuresMaster = await _unitOfWork.FeatureMasters.GetByIdAsync(id, cancellationToken);
            if (featuresMaster == null) throw new ResourceNotFoundException("FeaturesMaster not found.");

            featuresMaster.IsDeleted = true;
            featuresMaster.ModifiedAt = DateTime.UtcNow;
            featuresMaster.ModifiedBy = "System";

            _unitOfWork.FeatureMasters.Update(featuresMaster);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
