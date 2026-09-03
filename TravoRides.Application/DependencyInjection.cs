using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TravoRiders.Application.Interfaces;
using TravoRiders.Application.Services;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Services;
using TravoRides.Application.Services.Authentication;
using TravoRides.Domain.Entities;

namespace TravoRides.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(config => config.AddMaps(typeof(DependencyInjection).Assembly));

            services.AddScoped<ICabService, CabService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICategoryBasedService, CategoryBasedService>();
            services.AddScoped<IFeaturesMasterService, FeaturesMasterService>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<ISelfDriveService, SelfDriveService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IOtpVerificationService, EmailOtpVerificationService>();

            return services;
        }
    }
}
