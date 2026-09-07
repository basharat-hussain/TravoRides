using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.Review;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Mapping
{
    public class ReviewProfile :Profile
    {
        public ReviewProfile()
        {
            CreateMap<Review, ReviewDTO>();
            CreateMap<CreateReviewRequest, Review>();
            CreateMap<UpdateStatusRequest, Review>();
        }
    }
}
