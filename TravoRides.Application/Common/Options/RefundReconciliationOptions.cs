namespace TravoRides.Application.Common.Options
{
    public class RefundReconciliationOptions
    {
        public int BatchSize { get; set; } = 50;
        public int MaxRetries { get; set; } = 10;
    }
}

