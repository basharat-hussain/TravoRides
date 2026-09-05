using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Enums;

namespace TravoRides.Application.DTOs.BookingReport
{
    public class BookingReportDTO
    {
        public Guid Id { get; set; }

        public string BookingNo { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public DateTime BookingDate { get; set; }

        public DateTime TravelDate { get; set; }

        public string PickupLocation { get; set; } = string.Empty;

        public string DropLocation { get; set; } = string.Empty;

        public bool IsConfirmed { get; set; }

        // Successful/latest payment
        public decimal? PaymentAmount { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }

        public DateTime? PaidAt { get; set; }

        public int PaymentAttempts { get; set; }
    }
}
