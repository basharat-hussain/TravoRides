using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending = 1,
        Paid = 2,
        Failed = 3,
        Cancelled = 4,
       PartiallyRefunded = 5
    }
}
