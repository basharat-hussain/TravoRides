using System;

namespace TravoRides.Application.DTOs.CabFeatures
{
    public class UpdateCabFeaturesRequest
    {
        public Guid Id { get; set; }
        public Guid CabId { get; set; }
        public Guid FeatureId { get; set; }
    }
}
