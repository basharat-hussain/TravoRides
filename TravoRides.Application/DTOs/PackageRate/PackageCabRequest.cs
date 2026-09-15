using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.DTOs.PackageRate
{
    public class PackageCabRequest
    {
        public Guid CabId { get; set; }

        public decimal Rate { get; set; }

        public decimal? Discount { get; set; }
    }
}
