using TravoRides.Application.DTOs.Quote;
using TravoRides.Domain.Entities;
using AutoMapper;

namespace TravoRides.Application.Mapping
{
    public class QuoteProfile : Profile
    {
        public QuoteProfile()
        {
            CreateMap<Quote, QuoteDTO>();
            CreateMap<CreateQuoteRequest, Quote>();
            CreateMap<UpdateQuoteRequest, Quote>();
        }
    }
}
