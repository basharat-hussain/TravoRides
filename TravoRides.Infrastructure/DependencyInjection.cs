using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TravoRiders.Application.Common.Models;
using TravoRiders.Application.Common.Options;
using TravoRiders.Application.Interfaces.Services;
using TravoRiders.Application.Repositories;
using TravoRiders.Infrastructure.Authentication;
using TravoRiders.Infrastructure.Context;
using TravoRiders.Infrastructure.Repository;
using TravoRiders.Infrastructure.Services;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.Repositories;
using TravoRides.Infrastructure.Repository;

namespace TravoRides.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext, repositories, etc.
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<ICabRepository, CabRepository>();
            services.AddScoped<ISelfDriveRepository, SelfDriveRepository>();
            services.AddScoped<ICategoryBasedRepository, CategoryBasedRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IPackageRepository, PackageRepository>();
            services.AddScoped<IFeatureMasterRepository, FeatureMasterRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IEnquiryRepository, EnquiryRepository>();

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

            // Register UnitOfWork after repository registrations so DI validation can resolve repository implementations
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.Configure<FileStorageOptions>(options => configuration.GetSection("FileStorage").Bind(options));
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            return services;
        }
    }

}
