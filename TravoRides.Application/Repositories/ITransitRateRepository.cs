using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Repositories
{
    public interface ITransitRateRepository
    {
        Task<List<TransitRate>> GetByTransitIdAsync(Guid transitId, CancellationToken cancellationToken = default);
        Task<TransitRate?> GetByCabAndTransitAsync(Guid cabId,Guid transitId, CancellationToken cancellationToken = default);
    }
}
