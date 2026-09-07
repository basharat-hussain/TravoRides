using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.Enquirer;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Mapping
{
    public class EnquiryProfile : Profile
    {
        public EnquiryProfile()
        {
            CreateMap<Enquiry, EnquiryDTO>();
            CreateMap<CreateEnquiryRequest, Enquiry>();
        }
    }
}
