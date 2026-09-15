using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.DTOs.PackageRate
{
    public class UpdatePackageCabRequest
    {
        public decimal Rate { get; set; }

        public decimal? Discount { get; set; }
    }
}
