using AutoMapper;
using TravoRides.Application.DTOs.Payment;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Mapping
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Payment, PaymentDTO>()
                .ForMember(d => d.BookingNo, opt => opt.MapFrom(s => s.Booking != null ? s.Booking.BookingNo : null))
                .ForMember(d => d.CustomerName, opt => opt.MapFrom(s => s.Booking != null ? s.Booking.Name : null))
                .ForMember(d => d.CustomerEmail, opt => opt.MapFrom(s => s.Booking != null ? s.Booking.Email : null))
                .ForMember(d => d.CustomerPhone, opt => opt.MapFrom(s => s.Booking != null ? s.Booking.Phone : null))
                .ForMember(d => d.BookingTotalAmount, opt => opt.MapFrom(s => s.Booking != null ? s.Booking.TotalAmount : 0))
                .ForMember(d => d.TravelDate, opt => opt.MapFrom(s => s.Booking != null ? s.Booking.TravelDate : (DateTime?)null))
                .ForMember(d => d.PickupLocation, opt => opt.MapFrom(s => s.Booking != null ? s.Booking.PickupLocation : null))
                .ForMember(d => d.DropLocation, opt => opt.MapFrom(s => s.Booking != null ? s.Booking.DropLocation : null));
        }
    }
}

