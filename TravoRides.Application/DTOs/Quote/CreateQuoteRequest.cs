using System.ComponentModel.DataAnnotations;

namespace TravoRides.Application.DTOs.Quote
{
    public class CreateQuoteRequest
    {

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Passengers { get; set; }
        [Required]
        public string Requirements { get; set; }
    }
}
