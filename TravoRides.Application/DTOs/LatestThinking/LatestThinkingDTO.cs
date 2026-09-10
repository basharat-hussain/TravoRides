using System;

namespace TravoRides.Application.DTOs.LatestThinking
{
    public class LatestThinkingDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Subtitle { get; set; }
        public string ImageUrl { get; set; }
        public string? ImageAltText { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Slug { get; set; }
        public string? CanonicalUrl { get; set; }
        public string? Summary { get; set; }
        public string? KeyTakeaways { get; set; }
        public DateTime? PublishedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
