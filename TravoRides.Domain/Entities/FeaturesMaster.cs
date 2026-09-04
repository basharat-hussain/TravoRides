using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Common;

namespace TravoRides.Domain.Entities
{
    public class FeaturesMaster : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;

        public ICollection<CabFeatures> CabFeatures { get; set; } = new List<CabFeatures>();
    }
}
