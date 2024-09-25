namespace CinephoriaBackEnd.Models
{
    using AutoMapper;

    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<Room, RoomDto>()
                .ReverseMap();
        }
    }

}
