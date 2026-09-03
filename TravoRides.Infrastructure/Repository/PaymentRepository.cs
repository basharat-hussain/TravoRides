using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Infrastructure.Context;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Repository
{
    public class PaymentRepository : GenericRepository<Payment>
    {
        private readonly ApplicationDbContext _context;
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
            this._context = context;
        }


    }
}
