using TravoRides.Application.DTOs.Category;
using TravoRides.Application.DTOs.Common;
namespace TravoRides.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<PagedResponse<CategoryDTO>> GetAllAsync(SearchCategoryRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<CategoryDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
