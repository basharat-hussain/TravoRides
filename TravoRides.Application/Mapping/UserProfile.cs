using AutoMapper;
using TravoRides.Application.DTOs.Users;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserRequst, User>();
            CreateMap<User, UserOnlyResponse>();
            CreateMap<User, UserProfileResponse>();
                
        }
    }
}
