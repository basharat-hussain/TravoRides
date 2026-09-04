namespace TravoRides.Application.DTOs.Enquirer
{
    public class SearchEnquiryRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Keyword { get; set; }
    }
}
