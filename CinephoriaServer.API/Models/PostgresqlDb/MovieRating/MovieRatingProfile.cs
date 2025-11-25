using AutoMapper;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class MovieRatingProfile : Profile
    {
        public MovieRatingProfile()
        {
            // Mapping de CreateMovieRatingDto vers MovieRating
            CreateMap<CreateMovieRatingDto, MovieRating>()
                .ForMember(dest => dest.MovieId, opt => opt.MapFrom(src => src.MovieId))
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUserId))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.IsValidated, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Mapping de MovieRating vers MovieRatingDto
            CreateMap<MovieRating, MovieRatingDto>()
                .ForMember(dest => dest.MovieRatingId, opt => opt.MapFrom(src => src.MovieRatingId))
                .ForMember(dest => dest.MovieId, opt => opt.MapFrom(src => src.MovieId))
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUserId))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.IsValidated, opt => opt.MapFrom(src => src.IsValidated));

            // Mapping de UpdateMovieRatingDto vers MovieRating
            CreateMap<UpdateMovieRatingDto, MovieRating>()
                .ForMember(dest => dest.MovieRatingId, opt => opt.MapFrom(src => src.MovieRatingId))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
