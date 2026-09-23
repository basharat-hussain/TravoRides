using Microsoft.EntityFrameworkCore;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Infrastructure.Context;

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
        public IPaymentRepository Payments { get; }
        public IPaymentRefundRepository PaymentRefunds { get; }
        public IPaymentWebhookRepository PaymentWebhooks { get; }
        public IGenericRepository<Subscription> Subscriptions { get; }
        public IGenericRepository<Quote> Quotes { get; }
             
        public ITransitRepository Transit { get; }
        public IOtpVerificationRepository OtpVerifications { get; }
        public ICabRepository Cabs { get; }
        public ISelfDriveRepository SelfDrives { get; }
        public ICategoryRepository Categories { get; }

        public IFeatureMasterRepository FeatureMasters { get; }
        public IPackageRateRepository PackageRates { get; }
        public ITransitRateRepository TransitRates { get; }
        public IPackageRepository Packages { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            ICabRepository cabs,
            ISelfDriveRepository selfDrives, 
            IUserRepository user,
            IRefreshTokenRepository refreshTokens,
            IOtpVerificationRepository otpVerifications,
            ITransitRepository transit,
            ICategoryRepository category,
            IFeatureMasterRepository featureMasters,
            IPackageRepository packages,
            IEnquiryRepository enquiries,
            IReviewRepository reviews,
            IBookingRepository booking,
            ILatestThinkingRepository latestThinking,
            IPackageRateRepository packageRate,
            ITransitRateRepository transitRate,
            IPaymentRepository payments,
            IPaymentRefundRepository paymentRefunds,
            IPaymentWebhookRepository paymentWebhooks)
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
            Payments = payments;
            PaymentRefunds = paymentRefunds;
            PaymentWebhooks = paymentWebhooks;
            Subscriptions = new GenericRepository<Subscription>(context);
            Quotes = new GenericRepository<Quote>(context);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);

        public void Dispose()
            => _context.Dispose();

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transaction = _context.Database.CurrentTransaction;
            if (transaction != null)
                await transaction.CommitAsync(cancellationToken);
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transaction = _context.Database.CurrentTransaction;
            if (transaction != null)
                await transaction.RollbackAsync(cancellationToken);
        }
    }
}

