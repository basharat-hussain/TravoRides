using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Application.DTOs.Package;

namespace TravoRides.Application.DTOs.PackageRate
{
    public class MasterUpdate
    {
        // Section 1 - Package details
        public UpdatePackageRequest UpdatePackage { get; set; } = new();

        // Section 2 - Add cab
        public PackageCabRequest PackageCabRequest { get; set; } = new();

        // Available cabs for dropdown
        public List<CabDTO> AvailableCabs { get; set; } = new();

        // Section 3 - Already added cabs
        public List<PackageCabRateDTO> PackageCabRates { get; set; } = new();

        // Used when editing an existing package cab
        public UpdatePackageCabRequest UpdatePackageCab { get; set; } = new();
    }
}
