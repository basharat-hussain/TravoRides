using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.DTOs.CategoryBased
{
    public class SearchCategoryBasedRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Keyword { get; set; }
    }
}
