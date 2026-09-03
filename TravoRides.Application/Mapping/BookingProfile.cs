using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.BookingDTO;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Mapping
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<CreateBookingRequest, Booking>();

            CreateMap<UpdateBookingRequest, Booking>();

            CreateMap<Booking, BookingDTO>();
        }
    }
}
