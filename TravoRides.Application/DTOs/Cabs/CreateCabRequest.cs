using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Enums;

namespace TravoRides.Application.DTOs.Cabs
{
    public class CreateCabRequest
    {
        public Guid CategoryId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int SeatingCapacity { get; set; }

        public int LuggageCapacity { get; set; }
        public IFormFile Image { get; set; }
        public string Transmission { get; set; } = string.Empty;
        public FuelType Fuel { get; set; }
        public decimal PricePerDay { get; set; }
        public decimal Discount { get; set; }
    }
}
