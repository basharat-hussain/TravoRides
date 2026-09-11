using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Infrastructure.Context;

namespace TravoRides.Infrastructure.Repository
{
    public class PackageRateRepository : IPackageRateRepository
    {
        private readonly ApplicationDbContext _context;

        public PackageRateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PackageRate?> GetByCabAndPackageAsync(Guid cabId, Guid packageId,
            CancellationToken cancellationToken = default)
        {
            return await _context.PackageRates
        .Include(x => x.Cab)
        .Include(x => x.Package)
        .FirstOrDefaultAsync(x =>
            x.CabId == cabId &&
            x.PackageId == packageId &&
            !x.IsDeleted,
            cancellationToken);
        }
        public async Task<List<PackageRate>> GetByPackageIdAsync(Guid packageId, CancellationToken cancellationToken = default)
        {
            return await _context.PackageRates
                .Include(x => x.Cab)
                .Where(x =>
                    x.PackageId == packageId &&
                    !x.IsDeleted &&
                    !x.Cab.IsDeleted)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
