namespace CinephoriaBackEnd.Models
{
    using AutoMapper;

    public class ShowtimeProfile : Profile
    {
        public ShowtimeProfile()
        {
            CreateMap<Showtime, ShowtimeDto>()
                .ReverseMap();
        }
    }

}
