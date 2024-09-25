using AutoMapper;

namespace CinephoriaBackEnd.Models
{
    public class CinemaProfile : Profile
    {
        public CinemaProfile()
        {
            CreateMap<Cinema, CinemaDto>()
                .ReverseMap();
        }
    }
}
