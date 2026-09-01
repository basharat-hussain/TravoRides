using AutoMapper;
using TravoRides.Application.DTOs.CategoryBased;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Mapping
{
    public class CategoryBasedProfile : Profile
    {
        public CategoryBasedProfile()
        {
            CreateMap<CreateCategoryBasedRequest, CategoryBased>();
            CreateMap<UpdateCategoryBasedRequest, CategoryBased>();
            CreateMap<CategoryBased, CategoryBasedDTO>();
        }
    }
}
