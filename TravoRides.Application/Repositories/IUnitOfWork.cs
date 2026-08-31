using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
