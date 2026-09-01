using AlArwaSolutions.Infrastructure.Repository;
using AlArwaSolutions.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Application.Common.Options;
using TravoRiders.Application.Interfaces.Services;
using TravoRiders.Infrastructure.Context;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.Repositories;
using TravoRides.Infrastructure.Repository;

namespace TravoRides.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
        {
            // Register DbContext, repositories, etc.
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped<ICabRepository, CabRepository>();

            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.Configure<FileStorageOptions>(options => configuration.GetSection("FileStorage").Bind(options));

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<IFileUrlService, FileUrlService>();

            return services;
        }
    }

} 
