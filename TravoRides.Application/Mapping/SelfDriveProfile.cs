using AutoMapper;
using TravoRides.Application.DTOs.SelfDrive;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Mapping
{
    public class SelfDriveProfile : Profile
    {
        public SelfDriveProfile()
        {
            CreateMap<CreateSelfDriveRequest, SelfDrive>();
            CreateMap<UpdateSelfDriveRequest, SelfDrive>();
            CreateMap<SelfDrive, SelfDriveDTO>();
        }
    }
}
