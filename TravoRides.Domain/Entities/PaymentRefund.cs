using TravoRides.Domain.Common;
using TravoRides.Domain.Enums;

namespace TravoRides.Domain.Entities
{
    public class PaymentRefund : BaseEntity
    {
        public Guid PaymentId { get; set; }
        public string RefundNumber { get; set; } = null!;
        public string IdempotencyKey { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string? GatewayRefundId { get; set; }
        public PaymentRefundStatus Status { get; set; }
        public string? Reason { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? FailureReason { get; set; }
        public int RetryCount { get; set; }
        public DateTime? LastCheckedAt { get; set; }
        public Payment Payment { get; set; } = null!;
    }
}

