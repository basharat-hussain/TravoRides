using AutoMapper;
using TravoRides.Application.DTOs.CategoryBased;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Services
{
    public class CategoryBasedService : ICategoryBasedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryBasedService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<CategoryBasedDTO>> GetAllAsync(int pageNumber, int pageSize, string? keyword, CancellationToken cancellationToken = default)
        {
            var items = await _unitOfWork.CategoryBased.GetAllAsync(cancellationToken);
            var dtos = _mapper.Map<IEnumerable<CategoryBasedDTO>>(items);
            return new PagedResponse<CategoryBasedDTO>
            {
                Items = dtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = dtos.Count(),
                TotalPages = 1
            };
        }

        public async Task<CategoryBasedDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.CategoryBased.GetByIdAsync(id, cancellationToken);
            if (entity == null) return null;
            return _mapper.Map<CategoryBasedDTO>(entity);
        }

        public async Task<Guid> CreateAsync(CreateCategoryBasedRequest request, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<CategoryBased>(request);
            await _unitOfWork.CategoryBased.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }

        public async Task UpdateAsync(UpdateCategoryBasedRequest request, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<CategoryBased>(request);
            _unitOfWork.CategoryBased.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.CategoryBased.GetByIdAsync(id, cancellationToken);
            if (entity == null) return;
            entity.IsDeleted = true;
            _unitOfWork.CategoryBased.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
