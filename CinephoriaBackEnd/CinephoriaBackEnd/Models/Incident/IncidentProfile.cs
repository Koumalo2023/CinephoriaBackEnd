namespace CinephoriaBackEnd.Models
{
    using AutoMapper;

    public class IncidentProfile : Profile
    {
        public IncidentProfile()
        {
            // Mapping entre l'entité Incident et IncidentDto
            CreateMap<Incident, IncidentDto>()
                .ReverseMap();
        }
    }

}
