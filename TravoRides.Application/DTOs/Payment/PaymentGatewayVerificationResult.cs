namespace TravoRides.Application.DTOs.Payment
{
    public class PaymentGatewayVerificationResult
    {
        public bool Success { get; set; }
        public bool IsCaptured { get; set; }

        public string? GatewayTransactionId { get; set; }

        public decimal? Amount { get; set; }

        public string? Currency { get; set; }

        public string? ErrorCode { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
