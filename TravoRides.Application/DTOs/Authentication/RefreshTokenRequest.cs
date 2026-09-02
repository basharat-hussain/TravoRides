using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRiders.Application.DTOs.Authentication
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = null!;
    }
}
