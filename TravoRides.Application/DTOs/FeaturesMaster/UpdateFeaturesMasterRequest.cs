using System;

namespace TravoRides.Application.DTOs.FeaturesMaster
{
    public class UpdateFeaturesMasterRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}
