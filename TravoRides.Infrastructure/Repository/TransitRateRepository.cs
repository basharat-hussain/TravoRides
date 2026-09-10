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

        public async Task<TransitRate?> GetByCabAndTransitAsync(
            Guid cabId,
            Guid transitId,
            CancellationToken cancellationToken = default)
        {
            return await _context.TransitRates
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.CabId == cabId &&
                         x.TransitId == transitId,
                    cancellationToken);
        }
    }
}
