using System;
using TravoRides.Domain.Enums;

namespace TravoRides.Application.DTOs.Payment
{
    public class PaymentDTO
    {
        public Guid Id { get; set; }

        public string PaymentNumber { get; set; } = string.Empty;

        public Guid BookingId { get; set; }

        public string? BookingNo { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerEmail { get; set; }

        public string? CustomerPhone { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "INR";

        public PaymentStatus Status { get; set; }

        public string? GatewayName { get; set; }

        public string? GatewayTransactionId { get; set; }

        public string? GatewayOrderId { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? FailedAt { get; set; }

        public string? FailureReason { get; set; }

        public DateTime? RefundedAt { get; set; }

        public decimal RefundedAmount { get; set; }

        public int AttemptNumber { get; set; }

        public decimal BookingTotalAmount { get; set; }

        public DateTime? TravelDate { get; set; }

        public string? PickupLocation { get; set; }

        public string? DropLocation { get; set; }
    }
}

