using System;

namespace TravoRides.Application.DTOs.CategoryBased
{
    public class CategoryBasedDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        
    }
}
