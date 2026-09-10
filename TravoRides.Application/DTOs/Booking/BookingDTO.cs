using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Enums;

namespace TravoRides.Application.DTOs.BookingDTO
{
    public class BookingDTO
    {
        public Guid Id { get; set; }

        public string BookingNo { get; set; } = string.Empty;

        public Guid CabId { get; set; }

        public string? CabName { get; set; }

        public BookingType BookingType { get; set; }

        public Guid? TransitId { get; set; }

        public string? TransitName { get; set; }

        public Guid? PackageId { get; set; }

        public string? PackageName { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNo { get; set; } = string.Empty;

        public string WhatsApp { get; set; } = string.Empty;

        public DateTime TravelDate { get; set; }

        public string PickupLocation { get; set; } = string.Empty;

        public string DropLocation { get; set; } = string.Empty;

        public DateTime PickupTime { get; set; }

        public string Passengers { get; set; } = string.Empty;

        public string? Luggage { get; set; }

        public string? SpecialRequirements { get; set; }
        public bool IsConfirmed { get; set; }

        public decimal Rate { get; set; }
    }
}
