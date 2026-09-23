namespace TravoRides.Application.DTOs.Payment
{
    public class PaymentGatewayOrderResult
    {
        public bool Success { get; set; }

        public string? GatewayOrderId { get; set; }

        public string? GatewayName { get; set; }

        public string? ErrorCode { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
