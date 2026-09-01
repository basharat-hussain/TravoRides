using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Domain.Common;

namespace TravoRides.Domain.Entities
{
    public class Package : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public string Itinerary { get; set; } = string.Empty;

        public string Route { get; set; } = string.Empty;

        public string PlacesCovered { get; set; } = string.Empty;
        public string Inclusions { get; set; } = string.Empty;
        public string Duration { get; set; }

        public decimal Distance { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
