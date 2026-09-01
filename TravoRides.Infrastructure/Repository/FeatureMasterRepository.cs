using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Infrastructure.Context;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Repository
{
    public class FeatureMasterRepository : GenericRepository<FeaturesMaster>
    {
        private readonly ApplicationDbContext context;
        public FeatureMasterRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
