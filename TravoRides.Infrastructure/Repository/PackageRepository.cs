using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Infrastructure.Context;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Repository
{
    public class PackageRepository : GenericRepository<Package>
    {
        private readonly ApplicationDbContext context;
        public PackageRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
