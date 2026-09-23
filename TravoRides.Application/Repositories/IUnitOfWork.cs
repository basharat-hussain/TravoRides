using TravoRides.Domain.Entities;

namespace TravoRides.Application.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IBookingRepository Bookings { get; }
        IReviewRepository Reviews { get; }
        IEnquiryRepository Enquiries { get; }
        ILatestThinkingRepository LatestThinkings { get; }
        IPaymentRepository Payments { get; }
        IPaymentRefundRepository PaymentRefunds { get; }
        IPaymentWebhookRepository PaymentWebhooks { get; }
        IGenericRepository<Subscription> Subscriptions { get; }
        IGenericRepository<Quote> Quotes { get; }
        ITransitRepository Transit { get; }
        ITransitRateRepository TransitRates { get; }
        IPackageRateRepository PackageRates { get; }
        IOtpVerificationRepository OtpVerifications { get; }
        ICabRepository Cabs { get; }
        ISelfDriveRepository SelfDrives { get; }
        ICategoryRepository Categories { get; }
        IFeatureMasterRepository FeatureMasters { get; }
        IPackageRepository Packages { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}

