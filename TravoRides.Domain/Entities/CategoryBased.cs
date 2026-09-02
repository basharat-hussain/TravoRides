using System;
using System.Collections.Generic;
using System.Text;
using TravoRiders.Domain.Common;

namespace TravoRides.Domain.Entities
{
    public class CategoryBased : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal Discount { get; set; }
    }
}
