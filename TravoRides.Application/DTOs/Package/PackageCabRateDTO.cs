using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Domain.Enums;

namespace TravoRides.Application.DTOs.Package
{
    public class PackageCabRateDTO
    {
        public Guid CabId { get; set; }

        public string CabName { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public int SeatingCapacity { get; set; }

        public int LuggageCapacity { get; set; }

        public FuelType Fuel { get; set; }

        public string Transmission { get; set; } = string.Empty;

        public decimal Rate { get; set; }

        public decimal? Discount { get; set; }

        public decimal FinalRate { get; set; }
    }
}
