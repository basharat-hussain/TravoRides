using AutoMapper;
using TravoRides.Application.DTOs.SelfDrive;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Services
{
    public class SelfDriveService : ISelfDriveService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SelfDriveService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<SelfDriveDTO>> GetAllAsync(SearchSelfDriveRequest request, CancellationToken cancellationToken = default)
        {
            var items = await _unitOfWork.SelfDrives.GetAllAsync(cancellationToken);
            var dtos = _mapper.Map<IEnumerable<SelfDriveDTO>>(items);
            return new PagedResponse<SelfDriveDTO>
            {
                Items = dtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = dtos.Count(),
                TotalPages = 1
            };
        }

        public async Task<SelfDriveDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.SelfDrives.GetByIdAsync(id, cancellationToken);
            if (entity == null) return null;
            return _mapper.Map<SelfDriveDTO>(entity);
        }

        public async Task<Guid> CreateAsync(CreateSelfDriveRequest request, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<SelfDrive>(request);
            await _unitOfWork.SelfDrives.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }

        public async Task UpdateAsync(UpdateSelfDriveRequest request, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<SelfDrive>(request);
            _unitOfWork.SelfDrives.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.SelfDrives.GetByIdAsync(id, cancellationToken);
            if (entity == null) return;
            entity.IsDeleted = true;
            _unitOfWork.SelfDrives.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
