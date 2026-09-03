using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TravoRiders.Domain.Common;

namespace TravoRides.Domain.Entities
{
    public class Enquiry : BaseEntity
    {
        [Required(ErrorMessage = "Please enter your name")]
        [StringLength(100, ErrorMessage = "Name is too small", MinimumLength = 3)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
        [StringLength(100, ErrorMessage = "Email is too small", MinimumLength = 10)]
        public string Email { get; set; }


        [Required(ErrorMessage = "Please enter your phone number")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter numbers only")]
        [StringLength(12, ErrorMessage = "Phone should be 10 characters long", MinimumLength = 10)]
        public string Phone { get; set; }


        [Required(ErrorMessage = "Please enter subject")]
        [StringLength(200, ErrorMessage = "Suject is too small", MinimumLength = 5)]
        public string Subject { get; set; }


        [Required(ErrorMessage = "Please enter message")]
        [StringLength(2000, ErrorMessage = "Message is too small", MinimumLength = 10)]
        public string Message { get; set; }
    }
}
