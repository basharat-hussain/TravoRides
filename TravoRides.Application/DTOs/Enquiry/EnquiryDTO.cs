using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TravoRiders.Application.DTOs.Enquirer
{
    public class EnquiryDTO
    {
        public Guid Id { get; set; }

       
        public String Name { get; set; }

    
        public String Email { get; set; }

        public String Phone { get; set; }

        public String Subject { get; set; }

        public String Message { get; set; }
    }
}
