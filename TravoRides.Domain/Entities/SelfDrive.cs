using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Domain.Common;

namespace TravoRides.Domain.Entities
{
    public class SelfDrive : BaseEntity
    {
        public Guid CabId { get; set; }
        public Cab Cab { get; set; } = null!;

        public decimal PricePerDay { get; set; }

        public decimal Discount { get; set; }

    }
}
