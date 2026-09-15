using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.DTOs.TransitRate
{
    public class TransitCabRequest
    {
        public Guid CabId { get; set; }

        public decimal Rate { get; set; }

        public decimal? Discount { get; set; }
    }
}
