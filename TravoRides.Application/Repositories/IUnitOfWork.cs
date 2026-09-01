using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository User { get; }

        IRefreshTokenRepository RefreshTokens { get; }
        ICabRepository Cabs{ get; }
        ISelfDriveRepository SelfDrives { get; }
        ICabFeaturesRepository CabFeatures { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<FeaturesMaster> FeatureMasters { get; }
        IGenericRepository<Package> Packages { get; }
        IGenericRepository<CategoryBased> CategoryBased { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
