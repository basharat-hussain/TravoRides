using TravoRides.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace TravoRides.Domain.Entities
{
    public class LatestThinking : BaseEntity
    {
        [Required]
        public string Title { get; set; }

        public string? Subtitle { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public string? ImageAltText { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string Description { get; set; }

        // SEO
        public string? MetaTitle { get; set; }

        public string? MetaDescription { get; set; }

        public string? Slug { get; set; }

        public string? CanonicalUrl { get; set; }

        // AIO / AI-readable content
        public string? Summary { get; set; }

        public string? KeyTakeaways { get; set; }

        // Publishing
        public DateTime? PublishedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

    }
}
