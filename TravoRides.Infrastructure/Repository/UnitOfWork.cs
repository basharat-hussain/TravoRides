using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Application.Repositories;
using TravoRiders.Infrastructure.Context;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IUserRepository Users { get; }
        public IRefreshTokenRepository RefreshTokens { get; }

        public IBookingRepository Bookings { get; }
        public IGenericRepository<Payment> Payments { get; }
        public ICategoryBasedRepository CategoryBased { get; }
        public IOtpVerificationRepository OtpVerifications { get; }
        public ICabRepository Cabs { get; }
        public ISelfDriveRepository SelfDrives { get; }
        public ICategoryRepository Categories { get; }

        public IFeatureMasterRepository FeatureMasters { get; }

        public IPackageRepository Packages { get; }
         



        public UnitOfWork(ApplicationDbContext context, ICabRepository cabs, ISelfDriveRepository selfDrives, 
           IUserRepository user ,IRefreshTokenRepository refreshTokens,
            IOtpVerificationRepository otpVerifications,ICategoryBasedRepository categoryBased,
            ICategoryRepository category, IFeatureMasterRepository featureMasters, IPackageRepository packages
            )
        {
            _context = context;
            Cabs = cabs;
            SelfDrives = selfDrives;
            Users = user;
            RefreshTokens = refreshTokens;
            OtpVerifications = otpVerifications;
            CategoryBased = categoryBased;
            Categories = category;
            FeatureMasters = featureMasters;
            Packages = packages;

        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
           => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();

    }
}
