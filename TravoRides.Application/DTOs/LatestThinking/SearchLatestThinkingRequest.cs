using System;

namespace TravoRides.Application.DTOs.LatestThinking
{
    public class SearchLatestThinkingRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Keyword { get; set; }
        public string? Author { get; set; }
    }
}
