using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.DTOs.PackageRate
{
    public class AddCabsToPackageRequest
    {
        public List<PackageCabRequest> Cabs { get; set; } = new();
    }
}
