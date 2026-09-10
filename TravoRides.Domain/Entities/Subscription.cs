using TravoRides.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TravoRides.Domain.Entities
{
    public class Subscription : BaseEntity
    {
        
        public String Email { get; set; }
    }
}
