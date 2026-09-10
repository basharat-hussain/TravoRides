using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Infrastructure.Repository
{
    public class SubscriptionRepository : GenericRepository<Subscription>
    {
        private readonly ApplicationDbContext context;
        public SubscriptionRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
