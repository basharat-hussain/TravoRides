using TravoRides.Domain.Enums;

namespace TravoRides.Application.DTOs.Payment
{
    public class PaymentGatewayRefundResult
    {
        public PaymentGatewayResultStatus Status { get; set; }

        public string? GatewayRefundId { get; set; }

        public decimal? Amount { get; set; }

        public string? Currency { get; set; }

        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }

        public bool Success => Status == PaymentGatewayResultStatus.Success;
    }
}

