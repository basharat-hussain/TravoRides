using Microsoft.AspNetCore.Http;

namespace TravoRides.Application.DTOs.CategoryBased
{
    public class CreateCategoryBasedRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile ImageUrl { get; set; } 
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
    }
}
