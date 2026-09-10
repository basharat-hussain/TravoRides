using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Repositories
{
    public interface IPackageRateRepository
    {
        Task<PackageRate?> GetByCabAndPackageAsync(
            Guid cabId,
            Guid packageId,
            CancellationToken cancellationToken = default);
    }
}
