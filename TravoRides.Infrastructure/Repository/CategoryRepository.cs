using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Infrastructure.Context;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Repository
{
    public class CategoryRepository : GenericRepository<Category>
    {
        private readonly ApplicationDbContext context;
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
