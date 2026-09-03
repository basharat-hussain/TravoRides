using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TravoRiders.Application.DTOs.Review
{
    public class CreateReviewRequest
    {
        
        public string Name { get; set; }

        public string Address { get; set; }


        [Required(ErrorMessage = "Please enter your feedback")]
        [StringLength(2000, ErrorMessage = "feedback is too small", MinimumLength = 40)]
        public string Feedback { get; set; }


        [Required(ErrorMessage = "Please select rating")]
        [Range(1, 5, ErrorMessage = "select rating between 1-5")]
        public int Rating { get; set; }
        public string? ImageUrl { get; set; }
    }
}
