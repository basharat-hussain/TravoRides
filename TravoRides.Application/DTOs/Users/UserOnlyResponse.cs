using TravoRiders.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRiders.Application.DTOs.Users
{
    public class UserOnlyResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
    }
}
