using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.DTOs.Authentication
{
    public class ResendEmailVerificationRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
