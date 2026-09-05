using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.DTOs.BookingDTO
{
    public class UpdateBookingRequest
    {
        public Guid Id { get; set; }
        public string BookingNo { get; set; } = string.Empty;
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string WhatsApp { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime TravelDate { get; set; }
        public string PickupLocation { get; set; }
        public string DropLocation { get; set; }
        public DateTime PickupTime { get; set; }
        public string Passengers { get; set; } = string.Empty;
        public string? Luggage { get; set; } = string.Empty;
        public string? SpecialRequirements { get; set; } = string.Empty;
    }
}
