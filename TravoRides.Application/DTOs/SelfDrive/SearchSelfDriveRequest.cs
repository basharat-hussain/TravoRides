using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.DTOs.SelfDrive
{
    public class SearchSelfDriveRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Keyword { get; set; }
        public Guid? CabId { get; set; }
    }
}
