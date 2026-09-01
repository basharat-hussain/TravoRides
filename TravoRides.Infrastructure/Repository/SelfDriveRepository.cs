using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Infrastructure.Context;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Repository
{
    public class SelfDriveRepository : GenericRepository<SelfDrive>
    {
        private readonly ApplicationDbContext context;
        public SelfDriveRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
