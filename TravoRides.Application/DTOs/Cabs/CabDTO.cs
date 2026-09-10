using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.Category;
using TravoRides.Domain.Enums;

namespace TravoRides.Application.DTOs.Cabs
{
    public class CabDTO
    {
        public Guid Id { get; set; }
        public CategoryDTO Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SeatingCapacity { get; set; }
        public int LuggageCapacity { get; set; }

        public string ImageUrl { get; set; } = string.Empty;
        public string Transmission { get; set; } = string.Empty;
        public string Fuel { get; set; } = string.Empty;
        public decimal PricePerDay { get; set; }

        public decimal Discount { get; set; }
    }
}
