using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TravoRides.Application.DTOs.Subscription
{
    public class CreateSubscriptionRequest
    {
        [Required(ErrorMessage = "Please enter your email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
        [StringLength(100, ErrorMessage = "Email is too small", MinimumLength = 10)]
        public string Email { get; set; }
    }
}
