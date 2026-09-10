using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TravoRides.Domain.Enums;

namespace TravoRides.Application.DTOs.BookingDTO
{
    public class CreateBookingRequest
    {
        public string BookingNo { get; set; } = string.Empty;

        public Guid CabId { get; set; }

        public BookingType BookingType { get; set; }

        // Required only when BookingType = Transit
        public Guid? TransitId { get; set; }

        // Required only when BookingType = Package
        public Guid? PackageId { get; set; }

        [StringLength(100, ErrorMessage = "Name is too small", MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Required]
        public string PhoneNo { get; set; } = string.Empty;

        public string WhatsApp { get; set; } = string.Empty;

        public DateTime TravelDate { get; set; }

        public string PickupLocation { get; set; } = string.Empty;

        public string DropLocation { get; set; } = string.Empty;

        public DateTime PickupTime { get; set; }

        public string Passengers { get; set; } = string.Empty;

        public string? Luggage { get; set; }

        public string? SpecialRequirements { get; set; }
    }
}
