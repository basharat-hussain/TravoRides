using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Infrastructure.Context;

namespace TravoRides.Infrastructure
{
    public class DependencyInjection
    {
        public static void AddInfrastructureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add your infrastructure services here
            // For example, you can add your DbContext, repositories, etc.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            // Add other services as needed
            return;
        }
    }

}
