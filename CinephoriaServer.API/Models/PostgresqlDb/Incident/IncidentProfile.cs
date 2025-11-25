using AutoMapper;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class IncidentProfile : Profile
    {
        public IncidentProfile()
        {
            // --- Mapping de Incident vers IncidentDto ---
            CreateMap<Incident, IncidentDto>()
                .ForMember(dest => dest.TheaterName, opt => opt.MapFrom(src => src.Theater.Name))
                .ForMember(dest => dest.ReportedBy, opt => opt.MapFrom(src => $"{src.ReportedBy.FirstName} {src.ReportedBy.LastName}"))
                .ForMember(dest => dest.ResolvedBy, opt => opt.MapFrom(src => src.ResolvedBy != null ? $"{src.ResolvedBy.FirstName} {src.ResolvedBy.LastName}" : null))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls));

            // --- Mapping de CreateIncidentDto vers Incident ---
            // --- Mapping de CreateIncidentDto vers Incident ---
            CreateMap<CreateIncidentDto, Incident>()
                .ForMember(dest => dest.TheaterId, opt => opt.MapFrom(src => src.TheaterId))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls))
                .ForMember(dest => dest.ReportedById, opt => opt.MapFrom(src => src.ReportedBy)) 
                .ForMember(dest => dest.ResolvedById, opt => opt.Ignore()) 
                .ForMember(dest => dest.ReportedBy, opt => opt.Ignore()) 
                .ForMember(dest => dest.Theater, opt => opt.Ignore());


            // --- Mapping de UpdateIncidentDto vers Incident ---
            CreateMap<UpdateIncidentDto, Incident>()
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ResolvedAt, opt => opt.MapFrom(src => src.Status == IncidentStatus.Resolved ? DateTime.UtcNow : (DateTime?)null));
        }
    }

}
