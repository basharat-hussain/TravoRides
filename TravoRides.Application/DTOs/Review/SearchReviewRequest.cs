namespace TravoRiders.Application.DTOs.Review
{
    public class SearchReviewRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Keyword { get; set; }
    }
}
