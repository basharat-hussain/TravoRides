using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TravoRides.Domain.Common;
using TravoRides.Domain.Enums;

namespace TravoRides.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public string BookingNo { get; set; } = string.Empty;

        public Guid CabId { get; set; }

        public BookingType BookingType { get; set; }

        public Guid? TransitId { get; set; }

        public Guid? PackageId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string WhatsApp { get; set; } = string.Empty;

        public DateTime TravelDate { get; set; }

        public string PickupLocation { get; set; } = string.Empty;

        public string DropLocation { get; set; } = string.Empty;

        public DateTime PickupTime { get; set; }

        public string Passengers { get; set; } = string.Empty;

        public string? Luggage { get; set; }

        public bool IsConfirmed { get; set; }

        public string? SpecialRequirements { get; set; }

        // Final price at the time of booking
        public decimal Rate { get; set; }

        public Cab Cab { get; set; } = null!;

        public Transit? Transit { get; set; }

        public Package? Package { get; set; }

        public ICollection<Payment> Payments { get; set; }
            = new List<Payment>();
    }
}
