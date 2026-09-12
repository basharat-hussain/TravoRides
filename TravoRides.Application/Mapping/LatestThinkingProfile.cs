using TravoRides.Application.DTOs.LatestThinking;
using TravoRides.Domain.Entities;
using AutoMapper;

namespace TravoRides.Application.Mapping
{
    public class LatestThinkingProfile : Profile
    {
        public LatestThinkingProfile()
        {
            CreateMap<LatestThinking, LatestThinkingDTO>();
            CreateMap<CreateLatestThinkingRequest, LatestThinking>();
            CreateMap<UpdateLatestThinkingRequest, LatestThinking>();
        }
    }
}
