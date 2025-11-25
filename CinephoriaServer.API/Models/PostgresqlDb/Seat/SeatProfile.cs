using AutoMapper;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class SeatProfile : Profile
    {
        public SeatProfile()
        {
            // Mapping de Seat vers SeatDto
            CreateMap<Seat, SeatDto>()
                .ForMember(dest => dest.SeatNumber, opt => opt.MapFrom(src => src.SeatNumber));

            // Mapping de CreateSeatDto vers Seat
            CreateMap<CreateSeatDto, Seat>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));


            CreateMap<AddHandicapSeatDto, Seat>()
                .ForMember(dest => dest.SeatNumber, opt => opt.MapFrom(src => src.SeatNumber))
                .ForMember(dest => dest.IsAccessible, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<RemoveHandicapSeatDto, Seat>()
                .ForMember(dest => dest.SeatNumber, opt => opt.MapFrom(src => src.SeatNumber))
                .ForMember(dest => dest.IsAccessible, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Mapping de UpdateSeatDto vers Seat
            CreateMap<UpdateSeatDto, Seat>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
