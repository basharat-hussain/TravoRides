using System;

namespace TravoRides.Application.DTOs.Quote
{
    public class QuoteDTO
    {
        public Guid Id { get; set; }
       
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Passengers { get; set; }
        public string Requirements { get; set; }
    }
}
