using TravoRides.Domain.Enums;

namespace TravoRides.Application.DTOs.Payment
{
    public class PaymentVerificationResponse
    {
        public Guid PaymentId { get; set; }

        public string PaymentNumber { get; set; } = null!;

        public PaymentStatus Status { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; } = null!;
    }
}
