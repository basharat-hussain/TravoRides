using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.Common;

namespace TravoRides.Application.DTOs.BookingReport
{
    public class BookingReportResponse
    {
        public PagedResponse<BookingReportDTO> Bookings { get; set; } = null!;

        public BookingSummaryDTO Summary { get; set; } = null!;
    }
}
