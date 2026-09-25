using System;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Application.DTOs.Category;

namespace TravoRides.Application.DTOs.SelfDrive
{
    public class SelfDriveDTO
    {
        public Guid Id { get; set; }
      // public CabDTO Cab { get; set; }

        public CategoryDTO Category { get; set; }
        public decimal PricePerDay { get; set; }
        public decimal Discount { get; set; }
    }
}
