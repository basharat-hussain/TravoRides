using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Domain.Common;

namespace TravoRides.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<Cab> Cabs { get; set; } = new List<Cab>();
    }
}
