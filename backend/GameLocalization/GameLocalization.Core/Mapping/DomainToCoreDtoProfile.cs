using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.DTO.Languages;
using AutoMapper;
using GameLocalization.Core.DTO.Translations;

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

            CreateMap<Translation, TranslationDto>()
                .ForMember(d => d.KeyId,
                    m => m.MapFrom(s => s.LocalizationKeyId))
                .ForMember(d => d.LanguageCode,
                    m => m.MapFrom(s => s.Language.Code));
        }
    }
}
