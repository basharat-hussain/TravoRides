using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Infrastructure.Context;

namespace TravoRides.Infrastructure.Repository
{
    public class UnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
           => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();

    }
}
