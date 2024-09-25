namespace CinephoriaBackEnd.Models.Review
{
    using AutoMapper;

    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<Review, ReviewDto>()
                .ReverseMap();
        }
    }

}
