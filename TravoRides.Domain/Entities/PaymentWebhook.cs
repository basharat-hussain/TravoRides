using TravoRides.Domain.Common;

namespace TravoRides.Domain.Entities
{
    public class PaymentWebhook : BaseEntity
    {
        public string EventId { get; set; } = null!;
        public string EventType { get; set; } = null!;
        public string? PaymentGatewayId { get; set; }
        public string? GatewayOrderId { get; set; }
        public string? Payload { get; set; }
        public bool IsProcessed { get; set; }
        public DateTime ReceivedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? ErrorMessage { get; set; }
        public string HangfireJobId { get; set; } = null!;
        public DateTime RazorpayCreatedAt { get; set; }
    }
}

