using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TravoRides.Application.DTOs.Review
{
    public class ReviewDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }

        public string Feedback { get; set; }
        public bool IsActive { get; set; } = true;
        public int Rating { get; set; }
        public string? ImageUrl { get; set; }
    }
}
