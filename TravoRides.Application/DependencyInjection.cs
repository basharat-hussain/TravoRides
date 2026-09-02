using Microsoft.Extensions.DependencyInjection;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Services;

namespace TravoRides.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(config => config.AddMaps(typeof(DependencyInjection).Assembly));

            services.AddScoped<ICabService, CabService>();
            services.AddScoped<ICabFeaturesService, CabFeaturesService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICategoryBasedService, CategoryBasedService>();
            services.AddScoped<IFeaturesMasterService, FeaturesMasterService>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<ISelfDriveService, SelfDriveService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
