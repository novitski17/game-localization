using AutoMapper;
using GameLocalization.Api.Models.Requests.Auth;
using GameLocalization.Api.Models.Requests.Keys;
using GameLocalization.Api.Models.Requests.Languages;
using GameLocalization.Api.Models.Requests.LocalizationTable;
using GameLocalization.Api.Models.Requests.Translations;
using GameLocalization.Api.Models.Responses.Common;
using GameLocalization.Api.Models.Responses.Languages;
using GameLocalization.Api.Models.Responses.Table;
using GameLocalization.Api.Models.Responses.Translation;
using GameLocalization.Core.DTO.Auth;
using GameLocalization.Core.DTO.Common;
using GameLocalization.Core.DTO.Keys;
using GameLocalization.Core.DTO.Languages;
using GameLocalization.Core.DTO.Table;
using GameLocalization.Core.DTO.Translations;

namespace GameLocalization.Api.Helpers.Mappers
{
    public class CoreDtoToApiResponseProfile : Profile 
    {
        public CoreDtoToApiResponseProfile()
        {
            CreateMap<LanguageDto, LanguageResponse>();
            CreateMap<TranslationDto, TranslationResponse>()
                .ForMember(t => t.LocalizationKeyId, m => m.MapFrom(t => t.KeyId));
            CreateMap<LocalizationTableRowDto, LocalizationTableRowResponse>();

            CreateMap<CreateKeyRequest, CreateLocalizationKeyDto>()
                .ForMember(d => d.Key, opt => opt.MapFrom(s => s.Key.Trim()));

            CreateMap<UpdateTranslationRequest, UpdateTranslationDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResponse<>));

            CreateMap<LocalizationTablePageDto, LocalizationTablePageResponse>()
                .ForMember(d => d.Rows,
                    m => m.MapFrom(s => s.Rows));
            CreateMap<LocalizationTableQueryRequest, LocalizationTableQueryDto>()
                .ForMember(d => d.Search,
                    m => m.MapFrom(s => string.IsNullOrWhiteSpace(s.Search) ? null : s.Search.Trim()));
            CreateMap<LoginRequest, LoginDto>();
            CreateMap<RegisterRequest, RegisterDto>();
            CreateMap<CreateLanguageRequest, CreateLanguageDto>();
            CreateMap<UpdateLanguageRequest, UpdateLanguageDto>();
        }
    }
}
