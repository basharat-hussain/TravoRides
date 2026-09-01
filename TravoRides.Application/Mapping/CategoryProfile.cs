using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.Category;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Mapping
{
    public class CategoryProfile :Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDTO>();
            CreateMap<CreateCategoryRequest, Category>();
            CreateMap<UpdateCategoryRequest, Category>();
        }
    }
}
