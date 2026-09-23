using TravoRides.Domain.Enums;

namespace TravoRides.Application.DTOs.Payment
{
    public class RefundPaymentResponse
    {
        public Guid PaymentId { get; set; }

        public PaymentRefundStatus Status { get; set; }

        public decimal RefundedAmount { get; set; }

        public string? GatewayRefundId { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; } = null!;
    }
}
