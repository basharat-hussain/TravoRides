using AutoMapper;
using TravoRides.Application.DTOs.Package;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Services
{
    public class PackageService : IPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PackageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<PackageDTO>> GetAllAsync(int pageNumber, int pageSize, string? keyword, CancellationToken cancellationToken = default)
        {
            var items = await _unitOfWork.Packages.GetAllAsync(cancellationToken);
            var dtos = _mapper.Map<IEnumerable<PackageDTO>>(items);
            return new PagedResponse<PackageDTO>
            {
                Items = dtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = dtos.Count(),
                TotalPages = 1
            };
        }

        public async Task<PackageDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Packages.GetByIdAsync(id, cancellationToken);
            if (entity == null) return null;
            return _mapper.Map<PackageDTO>(entity);
        }

        public async Task<Guid> CreateAsync(CreatePackageRequest request, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<Package>(request);
            await _unitOfWork.Packages.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }

        public async Task UpdateAsync(PackageDTO request, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<Package>(request);
            _unitOfWork.Packages.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Packages.GetByIdAsync(id, cancellationToken);
            if (entity == null) return;
            entity.IsDeleted = true;
            _unitOfWork.Packages.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
