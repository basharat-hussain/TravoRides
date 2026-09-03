using AutoMapper;
using TravoRiders.Application.Common.Exceptions;
using TravoRides.Application.DTOs.Category;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.SelfDrive;
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
            // Defensive pagination
            if (request.PageNumber < 1)
                request.PageNumber = 1;

            if (request.PageSize < 1)
                request.PageSize = 8;

            if (request.PageSize > 100)
                request.PageSize = 100;

            var pagedResponse = await _unitOfWork.Categories
                .GetAllSearchAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.Keyword,
                    cancellationToken);

            var categoryDtos = _mapper.Map<IEnumerable<CategoryDTO>>(
                pagedResponse.Items);


            return new PagedResponse<CategoryDTO>
            {
                Items = categoryDtos,
                PageNumber = pagedResponse.PageNumber,
                PageSize = pagedResponse.PageSize,
                TotalCount = pagedResponse.TotalCount,
                TotalPages = pagedResponse.TotalPages
            };
        }

        public async Task<CategoryDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
            if (category == null) return null;
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<Guid> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationException("Category is required");

            var category = new Category
            {
              
                Name = request.Name?.Trim(),
                Description = request.Description?.Trim()
            };

            await _unitOfWork.Categories.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return category.Id;
        }

        public async Task UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken);
            if (category == null) throw new ResourceNotFoundException("Category not found.");

            category.Name = request.Name?.Trim();
            category.Description = request.Description?.Trim();
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
            if (category == null) throw new ResourceNotFoundException("Category not found.");

            category.IsDeleted = true;
            category.ModifiedAt = DateTime.UtcNow;
            category.ModifiedBy = "System";

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }


    }
}
