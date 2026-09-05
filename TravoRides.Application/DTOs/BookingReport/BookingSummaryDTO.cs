using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.DTOs.BookingReport
{
    public class BookingSummaryDTO
    {
        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int PendingBookings { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal SuccessfulAmount { get; set; }
        public decimal FailedAmount { get; set; }

        public List<BookingReportDTO> Bookings { get; set; } = [];
    }
}
