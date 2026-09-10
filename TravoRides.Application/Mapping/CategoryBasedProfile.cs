using AutoMapper;
using TravoRides.Application.DTOs.Transit;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Mapping
{
    public class TransitProfile : Profile
    {
        public TransitProfile()
        {
            CreateMap<CreateTransitRequest, Transit>();
            CreateMap<UpdateTransitRequest, Transit>();
            CreateMap<Transit, TransitDTO>();
        }
    }
}
