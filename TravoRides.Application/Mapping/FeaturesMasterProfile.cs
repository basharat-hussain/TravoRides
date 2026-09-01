using AutoMapper;
using TravoRides.Application.DTOs.FeaturesMaster;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Mapping
{
    public class FeaturesMasterProfile : Profile
    {
        public FeaturesMasterProfile()
        {
            CreateMap<CreateFeaturesMasterRequest, FeaturesMaster>();
            CreateMap<UpdateFeaturesMasterRequest, FeaturesMaster>();
            CreateMap<FeaturesMaster, FeaturesMasterDTO>();
        }
    }
}
