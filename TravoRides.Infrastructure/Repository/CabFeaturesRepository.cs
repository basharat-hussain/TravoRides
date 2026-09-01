using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Infrastructure.Context;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Repository
{
    public class CabFeaturesRepository : GenericRepository<CabFeatures>
    {
        private readonly ApplicationDbContext context;
        public CabFeaturesRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}