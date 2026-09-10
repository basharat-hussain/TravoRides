namespace TravoRides.Application.DTOs.Subscription
{
    public class SearchSubscriptionRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public string? Keyword { get; set; }
    }
}
