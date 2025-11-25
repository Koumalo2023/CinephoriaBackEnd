using AutoMapper;
using System.Collections.Generic;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            // Mapping de Movie vers MovieDto
            CreateMap<Movie, MovieDto>()
                .ForMember(dest => dest.PosterUrls, opt => opt.MapFrom(src => src.PosterUrls))
                .ForMember(dest => dest.Showtimes, opt => opt.MapFrom(src => src.Showtimes))
                .ForMember(dest => dest.MovieRatings, opt => opt.MapFrom(src => src.MovieRatings))
                .ForMember(dest => dest.Director, opt => opt.MapFrom(src => src.Director))
                .ForMember(dest => dest.Actors, opt => opt.MapFrom(src => src.Actors));

            // Mapping de CreateMovieDto vers Movie
            CreateMap<CreateMovieDto, Movie>()
                .ForMember(dest => dest.PosterUrls, opt => opt.MapFrom(src => src.PosterUrls))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Director, opt => opt.MapFrom(src => src.Director))
                .ForMember(dest => dest.Actors, opt => opt.MapFrom(src => src.Actors));

            // Mapping de UpdateMovieDto vers Movie
            CreateMap<UpdateMovieDto, Movie>()
                .ForMember(dest => dest.PosterUrls, opt => opt.MapFrom(src => src.PosterUrls))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Director, opt => opt.MapFrom(src => src.Director))
                .ForMember(dest => dest.Actors, opt => opt.MapFrom(src => src.Actors));

            // Mapping de Movie vers MovieDetailsDto
            CreateMap<Movie, MovieDetailsDto>()
                .ForMember(dest => dest.PosterUrls, opt => opt.MapFrom(src => src.PosterUrls))
                .ForMember(dest => dest.Showtimes, opt => opt.MapFrom(src => src.Showtimes))
                .ForMember(dest => dest.Ratings, opt => opt.MapFrom(src => src.MovieRatings))
                .ForMember(dest => dest.Director, opt => opt.MapFrom(src => src.Director))
                .ForMember(dest => dest.Actors, opt => opt.MapFrom(src => src.Actors));

            // Mapping de MovieRating vers MovieRatingDto
            CreateMap<MovieRating, MovieRatingDto>();

            // Mapping de Showtime vers ShowtimeDto
            CreateMap<Showtime, ShowtimeDto>();
        }
    }
}
