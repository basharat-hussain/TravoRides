using System.Text.Json.Serialization;

namespace TravoRides.Application.DTOs.Payment
{
    public class RazorpayWebhookRequest
    {
        [JsonPropertyName("entity")]
        public string? Entity { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }

        [JsonPropertyName("account_id")]
        public string? AccountId { get; set; }

        [JsonPropertyName("contains")]
        public List<string>? Contains { get; set; }

        [JsonPropertyName("payload")]
        public RazorpayWebhookPayload? Payload { get; set; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }
    }

    public class RazorpayWebhookPayload
    {
        [JsonPropertyName("payment")]
        public RazorpayWebhookPayment Payment { get; set; } = null!;
    }

    public class RazorpayWebhookPayment
    {
        [JsonPropertyName("entity")]
        public RazorpayPaymentEntity Entity { get; set; } = null!;
    }

    public class RazorpayPaymentEntity
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("entity")]
        public string? Entity { get; set; }

        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = null!;

        [JsonPropertyName("status")]
        public string Status { get; set; } = null!;

        [JsonPropertyName("order_id")]
        public string OrderId { get; set; } = null!;

        [JsonPropertyName("captured")]
        public bool Captured { get; set; }

        [JsonPropertyName("amount_refunded")]
        public long AmountRefunded { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("error_code")]
        public string? ErrorCode { get; set; }

        [JsonPropertyName("error_description")]
        public string? ErrorDescription { get; set; }

        [JsonPropertyName("error_source")]
        public string? ErrorSource { get; set; }

        [JsonPropertyName("error_step")]
        public string? ErrorStep { get; set; }

        [JsonPropertyName("error_reason")]
        public string? ErrorReason { get; set; }
    }
}

