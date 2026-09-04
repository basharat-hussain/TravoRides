using TravoRides.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravoRides.Domain.Entities
{
    public class Review : BaseEntity
    {
        public string Name { get; set; }
        
        public string Address { get; set; } 

        [StringLength(2000, ErrorMessage = "Feedback is too small", MinimumLength = 10)]
        public string Feedback { get; set; }
           
        public int Rating { get; set; }
        public string? ImageUrl { get; set; }
    }
}
