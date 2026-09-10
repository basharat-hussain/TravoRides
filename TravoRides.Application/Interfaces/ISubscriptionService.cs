using TravoRides.Application.DTOs.Common;

using TravoRides.Application.DTOs.Subscription;
using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.Interfaces
{
    public interface ISubscriptionService
    {
        Task<PagedResponse<SubscriptionDTO>> GetAllAsync(SearchSubscriptionRequest request, CancellationToken cancellationToken = default);

        Task<SubscriptionDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Guid> CreateAsync(CreateSubscriptionRequest request, CancellationToken cancellationToken = default);


        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
