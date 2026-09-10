using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace TravoRides.Application.DTOs.LatestThinking
{
    public class UpdateLatestThinkingRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Subtitle { get; set; }

        public IFormFile? ImageUrl { get; set; }

        public string? ImageAltText { get; set; }

        // Existing image URL to display current image in edit view
        public string? ImageUrlUrl { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string Description { get; set; }

        public string? MetaTitle { get; set; }

        public string? MetaDescription { get; set; }

        public string? Slug { get; set; }

        public string? CanonicalUrl { get; set; }

        public string? Summary { get; set; }

        public string? KeyTakeaways { get; set; }

        public DateTime? PublishedOn { get; set; }
    }
}
