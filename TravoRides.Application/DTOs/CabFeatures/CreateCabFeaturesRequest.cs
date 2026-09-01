using System;

namespace TravoRides.Application.DTOs.CabFeatures
{
    public class CreateCabFeaturesRequest
    {
        public Guid CabId { get; set; }
        public Guid FeatureId { get; set; }
    }
}
