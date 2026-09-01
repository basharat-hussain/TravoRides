using AutoMapper;
using TravoRides.Application.DTOs.Category;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<CategoryDTO>> GetAllAsync(SearchCategoryRequest request, CancellationToken cancellationToken = default)
        {
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 8;
            if (request.PageSize > 100) request.PageSize = 100;

            var paged = await _unitOfWork.Categories.GetAllSearchAsync(request.PageNumber, request.PageSize, request.Keyword, cancellationToken);

            var dtos = _mapper.Map<IEnumerable<CategoryDTO>>(paged.Items);

            return new PagedResponse<CategoryDTO>
            {
                Items = dtos,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages
            };
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var items = await _unitOfWork.Categories.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<CategoryDTO>>(items);
        }

        public async Task<CategoryDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
            if (entity == null) return null;
            return _mapper.Map<CategoryDTO>(entity);
        }

        public async Task<Guid> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<Category>(request);
            await _unitOfWork.Categories.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }

        public async Task UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<Category>(request);
            _unitOfWork.Categories.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
            if (entity == null) return;
            entity.IsDeleted = true;
            _unitOfWork.Categories.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
