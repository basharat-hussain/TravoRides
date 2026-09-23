using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Razorpay.Api;
using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.Common.Models;
using TravoRides.Application.Common.Options;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Interfaces.Payment;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.Repositories;
using TravoRides.Infrastructure.Authentication;
using TravoRides.Infrastructure.Context;
using TravoRides.Infrastructure.Payments.Razorpay;
using TravoRides.Infrastructure.Repository;
using TravoRides.Infrastructure.Services;

namespace TravoRides.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext, repositories, etc.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ICabRepository, CabRepository>();
            services.AddScoped<ISelfDriveRepository, SelfDriveRepository>();
            services.AddScoped<ITransitRepository, TransitRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IPackageRepository, PackageRepository>();
            services.AddScoped<IFeatureMasterRepository, FeatureMasterRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IEnquiryRepository, EnquiryRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<ILatestThinkingRepository, LatestThinkingRepository>();
            services.AddScoped<IPackageRateRepository, PackageRateRepository>();
            services.AddScoped<ITransitRateRepository, TransitRateRepository>();

            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaymentRefundRepository, PaymentRefundRepository>();
            services.AddScoped<IPaymentWebhookRepository, PaymentWebhookRepository>();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IOtpVerificationRepository, OtpVerificationRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<IFileUrlService, FileUrlService>();

            // Payment Services
            services.AddScoped<IPaymentGateway, RazorpayPaymentGateway>();
            services.AddScoped<IPaymentRefundService, PaymentRefundService>();
            services.AddScoped<IPaymentRefundReconciliationService, PaymentRefundReconcilationService>();
            services.AddScoped<IPaymentNumberGenerator, PaymentNumberGenerator>();
            services.AddScoped<IPaymentStateService, PaymentStateService>();
            services.AddScoped<IRazorPayPaymentWebhookService, RazorpayPaymentWebhookService>();

            // Razorpay Configuration & Client
            services.Configure<RazorpayOptions>(configuration.GetSection("PaymentGateway:Razorpay"));
            services.AddSingleton<RazorpayClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<RazorpayOptions>>().Value;

                if (string.IsNullOrWhiteSpace(settings.KeyId))
                    throw new ConflictException("Razorpay KeyId is not configured.");

                if (string.IsNullOrWhiteSpace(settings.KeySecret))
                    throw new ConflictException("Razorpay KeySecret is not configured.");

                return new RazorpayClient(settings.KeyId, settings.KeySecret);
            });

            // Hangfire Background Jobs
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

            services.AddHangfireServer();
            services.AddScoped<IJobScheduler, HangfireJobScheduler>();

            // Register UnitOfWork after repository registrations so DI validation can resolve repository implementations
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.Configure<FileStorageOptions>(options => configuration.GetSection("FileStorage").Bind(options));
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<PaymentOptions>(configuration.GetSection("Payment"));
            services.Configure<RefundReconciliationOptions>(configuration.GetSection("Payment:RefundReconciliation"));

            return services;
        }
    }
}

