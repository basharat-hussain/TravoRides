using System;

namespace TravoRides.Application.DTOs.Transit
{
    public class TransitDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        
    }
}
