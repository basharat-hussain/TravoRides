using TravoRides.Application.DTOs.Subscription;
using TravoRides.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.Mapping
{
    public class SubscriptionProfile : Profile
    {
        public SubscriptionProfile()
        {
            CreateMap<Subscription, SubscriptionDTO>();
            CreateMap<CreateSubscriptionRequest, Subscription>();
        }
    }
}
