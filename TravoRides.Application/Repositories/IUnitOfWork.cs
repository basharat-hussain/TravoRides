using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Infrastructure.Repository;

namespace TravoRides.Application.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }

        IRefreshTokenRepository RefreshTokens { get; }
        IBookingRepository Bookings { get; }
        IReviewRepository Reviews { get; }
        IEnquiryRepository Enquiries { get; }

        IGenericRepository<Payment> Payments { get; }
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
