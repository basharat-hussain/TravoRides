using Microsoft.AspNetCore.Http;
using System;

namespace TravoRides.Application.DTOs.CategoryBased
{
    public class UpdateCategoryBasedRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public decimal Discount { get; set; }

        public IFormFile? ImageFile { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
