using System;

namespace TravoRides.Application.DTOs.SelfDrive
{
    public class CreateSelfDriveRequest
    {
        public Guid CabId { get; set; }
        public decimal PricePerDay { get; set; }
        public decimal Discount { get; set; }
    }
}
