using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Domain.Common;

namespace TravoRides.Domain.Entities
{
    public class CabFeatures : BaseEntity
    {
        public Guid CabId { get; set; }
        public Cab Cab { get; set; } = null!;

        public Guid FeatureId { get; set; }
        public FeaturesMaster Feature { get; set; } = null!;
    }
}
