using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.DTOs.Package
{
    public class UpdatePackageRequest
    {
        public  Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string Itinerary { get; set; } = string.Empty;

        public string Route { get; set; } = string.Empty;

        public string PlacesCovered { get; set; } = string.Empty;
        public string Inclusions { get; set; } = string.Empty;
        public string Duration { get; set; }

        public decimal Distance { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }

        public IFormFile Image { get; set; }    
        public string? ImageUrl { get; set; } 
    }
}
