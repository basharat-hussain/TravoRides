namespace TravoRides.Infrastructure.Payments.Razorpay
{
    public class RazorpayOptions
    {
        public string KeyId { get; set; } = null!;

        public string KeySecret { get; set; } = null!;

        public string WebhookSecret { get; set; } = null!;
    }
}
