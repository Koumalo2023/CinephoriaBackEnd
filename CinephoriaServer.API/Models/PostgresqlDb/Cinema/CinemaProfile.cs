using AutoMapper;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class CinemaProfile : Profile
    {
        public CinemaProfile()
        {
            // Mapping de CreateCinemaDto vers Cinema
            CreateMap<CreateCinemaDto, Cinema>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Mapping de UpdateCinemaDto vers Cinema
            CreateMap<UpdateCinemaDto, Cinema>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Mapping de Cinema vers CinemaDto
            CreateMap<Cinema, CinemaDto>();
        }
    }
}
