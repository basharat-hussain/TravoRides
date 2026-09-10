using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TravoRides.Application.DTOs.Enquirer
{
    public class CreateEnquiryRequest
    {

        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required(ErrorMessage = "Please enter your phone number")]
        [Phone]
        public String Phone { get; set; }

        public string Subject { get; set; }


        [Required(ErrorMessage = "Please enter message")]
        [StringLength(2000, ErrorMessage = "Message is too small", MinimumLength = 10)]
        public string Message { get; set; }
    }
}
