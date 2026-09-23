namespace TravoRides.Application.DTOs.Payment
{
    public class VerifyPaymentRequest
    {
        public string GatewayOrderId { get; set; } = null!;

        public string GatewayTransactionId { get; set; } = null!;

        public string Signature { get; set; } = null!;
    }
}
