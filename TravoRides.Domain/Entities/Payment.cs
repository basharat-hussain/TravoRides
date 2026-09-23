using TravoRides.Domain.Common;
using TravoRides.Domain.Enums;

namespace TravoRides.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public string PaymentNumber { get; set; } = null!;
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public PaymentStatus Status { get; set; }

        public string? GatewayName { get; set; }
        public string? GatewayTransactionId { get; set; } 
        public string? GatewayOrderId { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? FailedAt { get; set; }
        public string? FailureReason { get; set; }
        public DateTime? RefundedAt { get; set; }
        public decimal RefundedAmount { get; set; }
        public int AttemptNumber { get; set; }

        public Booking Booking { get; set; } = null!;
        public ICollection<PaymentRefund> PaymentRefunds { get; set; } = new List<PaymentRefund>();
    }
}

