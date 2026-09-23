namespace TravoRides.Application.DTOs.Payment
{
    public class PaymentGatewayVerificationRequest
    {
        public string GatewayOrderId { get; set; } = null!;

        public string GatewayTransactionId { get; set; } = null!;

        public string Signature { get; set; } = null!;
    }
}
