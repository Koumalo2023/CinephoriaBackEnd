namespace CinephoriaBackEnd.Models
{
    using AutoMapper;

    public class AppUserProfile : Profile
    {
        public AppUserProfile()
        {
            CreateMap<AppUser, AppUserDto>()
                .ReverseMap();
        }
    }

}
