using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Repositories
{
    public interface ITransitRateRepository
    {
        Task<TransitRate?> GetByCabAndTransitAsync(
            Guid cabId,
            Guid transitId,
            CancellationToken cancellationToken = default);
    }
}
