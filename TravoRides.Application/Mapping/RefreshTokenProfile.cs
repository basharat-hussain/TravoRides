using AutoMapper;
using TravoRiders.Domain.Entities;
using TravoRides.Application.DTOs.RefreshTokens;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Mapping
{
    public class RefreshTokenProfile : Profile
    {
        public RefreshTokenProfile()
        {
            CreateMap<RefreshToken, RefreshTokenDTO>();
        }
    }
}
