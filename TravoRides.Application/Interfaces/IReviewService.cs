
using TravoRiders.Application.DTOs.Review;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.Common;

namespace TravoRiders.Application.Interfaces
{
    public interface IReviewService
    {
        Task<PagedResponse<ReviewDTO>> GetAllAsync(SearchReviewRequest request, CancellationToken cancellationToken = default);

        Task<ReviewDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Guid> CreateAsync(CreateReviewRequest request, CancellationToken cancellationToken = default);


        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResponse<ReviewDTO>> GetAllApprovedAsync(SearchReviewRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(UpdateReviewRequest request, CancellationToken cancellationToken);

        // Toggle or set review active status
        Task ToggleStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken = default);
    }
}
