using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TravoRiders.Domain.Common;

namespace TravoRides.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public string BookingNo { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please enter your name")]
        [StringLength(100, ErrorMessage = "Name is too small", MinimumLength = 3)]
        public String Name { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
        [StringLength(100, ErrorMessage = "Email is too small", MinimumLength = 10)]
        public String Email { get; set; }


        [Required(ErrorMessage = "Please enter your phone number")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter numbers only")]
        [StringLength(12, ErrorMessage = "Phone should be 10 characters long", MinimumLength = 10)]
        public String Phone { get; set; }
        public string WhatsApp { get; set; } = string.Empty;
        public DateTime TravelDate { get; set; }
        public string PickupLocation { get; set; }
        public string DropLocation { get; set; } 
        public string PickupTime { get; set; } = string.Empty;
        public string Passengers { get; set; } = string.Empty;
        public string? Luggage { get; set; } = string.Empty;
        public string? SpecialRequirements { get; set; } = string.Empty;

        public Payment Payment { get; set; } = null!;
    }
}
