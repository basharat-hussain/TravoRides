using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Infrastructure.Context;

namespace TravoRides.Infrastructure.Repository
{
    public class TransitRateRepository : ITransitRateRepository
    {
        private readonly ApplicationDbContext _context;

        public TransitRateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TransitRate?> GetByCabAndTransitAsync(Guid cabId,Guid transitId,
            CancellationToken cancellationToken = default)
        {
         return await _context.TransitRates
      .Include(x => x.Cab)
      .Include(x => x.Transit)
      .FirstOrDefaultAsync(x =>
          x.CabId == cabId &&
          x.TransitId == transitId &&
          !x.IsDeleted,
          cancellationToken);
        }

        public async Task<List<TransitRate>> GetByTransitIdAsync(Guid transitId, CancellationToken cancellationToken = default)
        {
            return await _context.TransitRates
                .Include(x => x.Cab)
                .Where(x =>
                    x.TransitId == transitId &&
                    !x.IsDeleted &&
                    !x.Cab.IsDeleted)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
