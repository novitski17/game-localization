using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.DTO.Languages;
using AutoMapper;

namespace GameLocalization.Core.Mapping
{
    public class DomainToCoreDtoProfile : Profile
    {
        public DomainToCoreDtoProfile()
        {
            CreateMap<Language, LanguageDto>();

            CreateMap<CreateLanguageDto, Language>()
                .ForMember(d => d.Code, m => 
                    m.MapFrom(s => s.Code.Trim().ToLowerInvariant()))
                .ForMember(d => d.Name, m => 
                    m.MapFrom(s => s.Name.Trim()));
        }
    }
}
