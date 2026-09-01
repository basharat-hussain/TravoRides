using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Domain.Common;

namespace TravoRides.Domain.Entities
{
    public class Cab : BaseEntity
    {
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LuggageCapacity { get; set; } = string.Empty;
        public int SeatingCapacity { get; set; }
        public decimal PricePerDay { get; set; }
        public int Fuel { get; set; }

        public string Transmission { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public SelfDrive? SelfDrive { get; set; }

        public ICollection<CabFeatures> CabFeatures { get; set; } = new List<CabFeatures>();


    }
}
