using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application
{
    public class DependencyInjection
    {
        public static void AddApplicationServices(IServiceCollection services)
        {
            // Add your application services here
            // For example, you can add your MediatR handlers, validators, etc.
            // services.AddMediatR(typeof(DependencyInjection).Assembly);
            // services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
            return;
        }
    }
}
