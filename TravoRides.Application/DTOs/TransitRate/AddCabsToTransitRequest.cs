using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.DTOs.TransitRate
{
    public class AddCabsToTransitRequest
    {
        public List<TransitCabRequest> Cabs { get; set; } = new();

    }
}
