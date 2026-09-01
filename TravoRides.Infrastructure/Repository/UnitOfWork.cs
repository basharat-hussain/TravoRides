using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Infrastructure.Context;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Repository
{
    public class UnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ICabRepository Cabs { get; }

        public IGenericRepository<Category> Categories { get; }

        public UnitOfWork(ApplicationDbContext context, ICabRepository cabs)
        {
            _context = context;
            Cabs = cabs;
            Categories = new GenericRepository<Category>(_context);
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
           => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();

    }
}
