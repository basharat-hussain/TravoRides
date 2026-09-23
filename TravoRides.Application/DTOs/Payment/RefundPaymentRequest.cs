namespace TravoRides.Application.DTOs.Payment
{
    public class RefundPaymentRequest
    {
        public decimal Amount { get; set; }

        public string? Reason { get; set; }
    }
}
