using System;
using TravoRides.Domain.Enums;

namespace TravoRides.Application.DTOs.Payment
{
    public class SearchPaymentRequest
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Keyword { get; set; }

        // Filter presets: "today", "thisweek", "thismonth", "past3months", "thisyear", "custom"
        public string? DateFilter { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public PaymentStatus? Status { get; set; }
    }
}

