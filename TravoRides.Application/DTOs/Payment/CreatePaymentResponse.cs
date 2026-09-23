namespace TravoRides.Application.DTOs.Payment
{
    public class CreatePaymentResponse
    {
        public Guid PaymentId { get; set; }

        public string PaymentNumber { get; set; } = null!;

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "INR";

        public string GatewayName { get; set; } = null!;

        public string GatewayOrderId { get; set; } = null!;

        public string GatewayKeyId { get; set; } = null!;
    }
}
