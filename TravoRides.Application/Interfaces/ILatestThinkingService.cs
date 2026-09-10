using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.LatestThinking;
using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.Interfaces
{
    public interface ILatestThinkingService
    {
        Task<PagedResponse<LatestThinkingDTO>> GetAllAsync(SearchLatestThinkingRequest request, CancellationToken cancellationToken = default);
        Task<LatestThinkingDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> CreateAsync(CreateLatestThinkingRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(UpdateLatestThinkingRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
