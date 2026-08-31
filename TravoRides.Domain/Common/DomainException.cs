using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRiders.Domain.Common
{
    public sealed class DomainException(string message) : Exception(message)
    {
    }
}
