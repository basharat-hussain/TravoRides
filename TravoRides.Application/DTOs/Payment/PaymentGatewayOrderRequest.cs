namespace TravoRides.Application.DTOs.Payment
{
    public class PaymentGatewayOrderRequest
    {
        public string PaymentNumber { get; set; } = null!;

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "INR";

        public string Description { get; set; } = string.Empty;
    }
}
