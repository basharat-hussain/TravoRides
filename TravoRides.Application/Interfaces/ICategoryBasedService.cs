using TravoRides.Application.DTOs.CategoryBased;
using TravoRides.Application.DTOs.Common;

namespace TravoRides.Application.Interfaces
{
    public interface ICategoryBasedService
    {
        Task<PagedResponse<CategoryBasedDTO>> GetAllAsync(SearchCategoryBasedRequest request, CancellationToken cancellationToken = default);
        Task<CategoryBasedDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> CreateAsync(CreateCategoryBasedRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(UpdateCategoryBasedRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
