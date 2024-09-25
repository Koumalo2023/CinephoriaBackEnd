namespace CinephoriaBackEnd.Models
{
    using AutoMapper;

    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            CreateMap<Movie, MovieDto>()
                .ReverseMap();
        }
    }

}
