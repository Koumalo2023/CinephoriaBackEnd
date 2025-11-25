using AutoMapper;
using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.DTOs.Showtime;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class ShowtimeProfile : Profile
    {
        public ShowtimeProfile()
        {
            // Mapping de CreateShowtimeDto vers Showtime
            CreateMap<CreateShowtimeDto, Showtime>()
                .ForMember(dest => dest.Price, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EnumConfig.ShowtimeStatus.Upcoming))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Mapping de UpdateShowtimeDto vers Showtime
            CreateMap<UpdateShowtimeDto, Showtime>()
                .ForMember(dest => dest.Price, opt => opt.Ignore()) // Le prix sera recalculé dans le service
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Mapping de Showtime vers ShowtimeDto
            CreateMap<Showtime, ShowtimeDto>();

            // Mapping de Showtime vers ShowtimeStatusDto
            CreateMap<Showtime, ShowtimeStatusDto>()
                .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title))
                .ForMember(dest => dest.TheaterName, opt => opt.MapFrom(src => src.Theater.Name))
                .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => GetStatusDisplay(src.Status)))
                .ForMember(dest => dest.AvailableSeats, opt => opt.MapFrom(src => src.Theater.SeatCount - src.Reservations.Count))
                .ForMember(dest => dest.TotalSeats, opt => opt.MapFrom(src => src.Theater.SeatCount));
        }

        private static string GetStatusDisplay(EnumConfig.ShowtimeStatus status)
        {
            return status switch
            {
                EnumConfig.ShowtimeStatus.Upcoming => "À venir",
                EnumConfig.ShowtimeStatus.Ongoing => "En cours",
                EnumConfig.ShowtimeStatus.Completed => "Terminée",
                EnumConfig.ShowtimeStatus.Cancelled => "Annulée",
                _ => "Inconnu"
            };
        }
    }

}
