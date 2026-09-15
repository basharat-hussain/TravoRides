using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.DTOs.TransitRate
{
    public class UpdateTransitCabRequest
    {
        public decimal Rate { get; set; }

        public decimal? Discount { get; set; }
    }
}
