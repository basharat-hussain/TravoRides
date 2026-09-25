using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.BookingDTO;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Domain.Entities;

using TravoRides.Domain.Enums;

namespace TravoRides.Application.Mapping
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<CreateBookingRequest, Booking>();

            CreateMap<UpdateBookingRequest, Booking>();

            CreateMap<Booking, BookingDTO>()
                .ForMember(d => d.PhoneNo, opt => opt.MapFrom(s => s.Phone))
                .ForMember(d => d.Rate, opt => opt.MapFrom(s => s.TotalAmount))
                .ForMember(d => d.TotalAmount, opt => opt.MapFrom(s => s.TotalAmount))
                .ForMember(d => d.CabName, opt => opt.MapFrom(s => s.Cab != null ? s.Cab.Name : null))
                .ForMember(d => d.IsPaid, opt => opt.MapFrom(s => s.Payments != null && s.Payments.Any(p => p.Status == PaymentStatus.Paid)))
                .ForMember(d => d.PaymentStatus, opt => opt.MapFrom(s => s.Payments != null ? s.Payments.OrderByDescending(p => p.AttemptNumber).Select(p => (PaymentStatus?)p.Status).FirstOrDefault() : null))
                .ForMember(d => d.PaidAmount, opt => opt.MapFrom(s => s.Payments != null ? s.Payments.Where(p => p.Status == PaymentStatus.Paid).OrderByDescending(p => p.AttemptNumber).Select(p => (decimal?)p.Amount).FirstOrDefault() : null))
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => s.CreatedAt));
        }
    }
}
