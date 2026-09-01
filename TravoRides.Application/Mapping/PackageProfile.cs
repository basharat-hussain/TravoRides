using AutoMapper;
using TravoRides.Application.DTOs.Package;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Mapping
{
    public class PackageProfile : Profile
    {
        public PackageProfile()
        {
            CreateMap<CreatePackageRequest, Package>();
            CreateMap<UpdatePackageRequest, Package>();
            CreateMap<Package, PackageDTO>();
        }
    }
}
