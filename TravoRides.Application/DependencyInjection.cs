using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Application.Interfaces.Services;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;
using TravoRides.Application.Services;

namespace TravoRides.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(config => config.AddMaps(typeof(DependencyInjection).Assembly));

            services.AddScoped<ICabService, CabService>();
            return services;
        }
    }
}
