using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Common;

namespace TravoRides.Domain.Entities
{
    public class TransitRate : BaseEntity
    {
        public Guid CabId { get; set; }

        public Guid TransitId { get; set; }

        public decimal Rate { get; set; }
        public decimal? Discount { get; set; }

        public Cab Cab { get; set; } = null!;
        public Transit Transit { get; set; } = null!;
    }
}
