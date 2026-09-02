using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }

        IRefreshTokenRepository RefreshTokens { get; }

        ICategoryBasedRepository CategoryBased { get; }
        IOtpVerificationRepository OtpVerifications { get; }
        ICabRepository Cabs{ get; }
        ISelfDriveRepository SelfDrives { get; }
        ICategoryRepository Categories { get; }
        IFeatureMasterRepository FeatureMasters { get; }
        IPackageRepository Packages { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
