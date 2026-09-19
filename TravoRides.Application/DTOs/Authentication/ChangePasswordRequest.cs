using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.DTOs.Authentication
{
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
