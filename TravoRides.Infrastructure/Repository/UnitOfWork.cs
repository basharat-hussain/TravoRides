using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.Repositories;
using TravoRides.Infrastructure.Context;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IUserRepository Users { get; }
        public IRefreshTokenRepository RefreshTokens { get; }
  public ILatestThinkingRepository LatestThinkings { get; }
        public IEnquiryRepository Enquiries { get; }
        public IReviewRepository Reviews { get; }
        public IBookingRepository Bookings { get; }
        public IGenericRepository<Payment> Payments { get; }
        public IGenericRepository<Subscription> Subscriptions { get; }
        public ITransitRepository Transit { get; }
        public IOtpVerificationRepository OtpVerifications { get; }
        public ICabRepository Cabs { get; }
        public ISelfDriveRepository SelfDrives { get; }
        public ICategoryRepository Categories { get; }

        public IFeatureMasterRepository FeatureMasters { get; }
        public IPackageRateRepository PackageRates { get; }
        public ITransitRateRepository TransitRates { get; }
        public IPackageRepository Packages { get; }
         



        public UnitOfWork(ApplicationDbContext context, ICabRepository cabs, ISelfDriveRepository selfDrives, 
           IUserRepository user ,IRefreshTokenRepository refreshTokens,
            IOtpVerificationRepository otpVerifications,ITransitRepository transit,
            ICategoryRepository category, IFeatureMasterRepository featureMasters, IPackageRepository packages
            ,IEnquiryRepository enquiries, IReviewRepository reviews,IBookingRepository booking,
            ILatestThinkingRepository latestThinking,IPackageRateRepository packageRate,
            ITransitRateRepository transitRate)
        {
            _context = context;
            Cabs = cabs;
            SelfDrives = selfDrives;
            Users = user;
            RefreshTokens = refreshTokens;
            OtpVerifications = otpVerifications;
            Transit = transit;
            Categories = category;
            FeatureMasters = featureMasters;
            Packages = packages;
            Enquiries = enquiries;
            Reviews = reviews;
            Bookings = booking;
            LatestThinkings = latestThinking;
            PackageRates = packageRate;
            TransitRates = transitRate;
            Payments = new GenericRepository<Payment>(context);
            Subscriptions = new GenericRepository<Subscription>(context);
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
           => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();

    }
}
