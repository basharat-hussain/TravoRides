namespace TravoRides.Application.DTOs.Payment
{
    public class PaymentGatewayRefundRequest
    {
        public string GatewayTransactionId { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string? Reason { get; set; }
    }
}

