using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Domain.Common;
using TravoRides.Domain.Enums;

namespace TravoRides.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public Guid BookingId { get; set; }
        public Guid PaymentNo { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public PaymentStatus PaymentStatus { get; set; }

        public string? GatewayName { get; set; }
        public string? GateTransId { get; set; } 
        public string? GatewayOrderId { get; set; }
        public DateTime? FailedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? FailureReason { get; set; }
        public int AttemptNumber { get; set; }

        public Booking Booking { get; set; } = null!;
    }
}
