using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Common;

namespace TravoRides.Domain.Entities
{
    public class PackageRate :BaseEntity
    {
        public Guid CabId { get; set; }

        public Guid PackageId { get; set; }

        public decimal Rate { get; set; }
        public decimal? Discount { get; set; }

        public Cab Cab { get; set; } = null!;
        public Package Package { get; set; } = null!;
    }
}
