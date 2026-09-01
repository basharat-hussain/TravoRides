using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.DTOs.Category
{
    public class CategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
