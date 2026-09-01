using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICabRepository Cabs{ get; }

        IGenericRepository<Category> Categories { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
