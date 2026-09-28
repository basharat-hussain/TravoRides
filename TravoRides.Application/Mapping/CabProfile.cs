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

            CreateMap<Cab, CabDTO>()
                .ForMember(c => c.Fuel, opt => opt.MapFrom(src => src.Fuel.ToString()))
                .ForMember(c => c.Features, opt => opt.MapFrom(src =>
                    src.CabFeatures != null
                        ? src.CabFeatures
                            .Where(cf => !cf.IsDeleted && cf.Feature != null && !cf.Feature.IsDeleted)
                            .Select(cf => cf.Feature)
                        : Enumerable.Empty<FeaturesMaster>()));

        }
    }
}
