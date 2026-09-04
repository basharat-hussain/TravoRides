using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.DTOs.Authentication
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = null!;
    }
}
