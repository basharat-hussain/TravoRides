using Microsoft.AspNetCore.Http;

namespace TravoRides.Application.DTOs.Transit
{
    public class CreateTransitRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile Image { get; set; } 
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
    }
}
