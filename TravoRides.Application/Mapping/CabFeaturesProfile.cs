using AutoMapper;
using TravoRides.Application.DTOs.CabFeatures;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Mapping
{
    public class CabFeaturesProfile : Profile
    {
        public CabFeaturesProfile()
        {
            CreateMap<CreateCabFeaturesRequest, CabFeatures>();
            CreateMap<UpdateCabFeaturesRequest, CabFeatures>();
            CreateMap<CabFeatures, CabFeaturesDTO>();
        }
    }
}
