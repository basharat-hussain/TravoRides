namespace TravoRides.Application.Common.Options
{
    public class PaymentOptions
    {
        // Provider name e.g., "Stripe", "PayPal"
        public string Provider { get; set; } = string.Empty;

        // Public API key (if applicable)
        public string ApiKey { get; set; } = string.Empty;

        // Secret key used for server operations
        public string SecretKey { get; set; } = string.Empty;

        // Webhook secret (if using webhooks)
        public string WebhookSecret { get; set; } = string.Empty;

        // Currency code e.g., "USD"
        public string Currency { get; set; } = "USD";

        // Mode: Live or Test
        public string Mode { get; set; } = "Test";

        // URLs for redirects (if applicable)
        public string ReturnUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
    }
}
