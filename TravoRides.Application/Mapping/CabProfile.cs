using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Application.DTOs.Category;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Mapping
{
    public class CabProfile : Profile
    {
        public CabProfile()
        {
            CreateMap<CreateCabRequest, Cab>();

            CreateMap<UpdateCabRequest, Cab>();

            CreateMap<Cab, CabDTO>();

        }
    }
}
