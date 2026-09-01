using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.DTOs.Category
{
    public class UpdateCategoryRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
