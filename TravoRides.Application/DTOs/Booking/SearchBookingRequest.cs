using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.DTOs.BookingDTO
{
    public class SearchBookingRequest
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Keyword { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
