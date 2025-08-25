using FluentResults;
using FluentValidation;
using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.DTO.Common;
using GameLocalization.Core.DTO.Table;
using GameLocalization.Core.Extensions;
using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Core.Interfaces.Services;
using GameLocalization.Core.Validation;

namespace GameLocalization.Core.Services
{
    public class LocalizationTableService : ILocalizationTableService
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly ILocalizationKeyRepository _keyRepository;
        private readonly IValidator<LocalizationTableQueryDto> _validator;

        public LocalizationTableService(
            ILanguageRepository languagesRepository,
            ILocalizationKeyRepository keysRepository,
            IValidator<LocalizationTableQueryDto> validator)
        {
            _languageRepository = languagesRepository ?? throw new ArgumentNullException(nameof(languagesRepository));
            _keyRepository = keysRepository ?? throw new ArgumentNullException(nameof(keysRepository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task<Result<LocalizationTablePageDto>> GetAsync(LocalizationTableQueryDto query, CancellationToken ct)
        {
            var validation = await _validator.ValidateAsync(query, ct);

            if (!validation.IsValid)
            {
                return Result.Fail(ValidationErrorsHelper.ToValidationAppError(validation));
            }

            var languagesCode = await LoadLanguageCodesAsync(query.IncludeDisabledLanguages, ct);

            var page = await _keyRepository
                .GetPageWithTranslationsAsync(
                    query.Page,
                    query.PageSize,
                    query.Search.NormalizeCode(),
                    query.IncludeDisabledLanguages,
                    ct);

            var rows = BuildRows(page.Items, languagesCode);

            var result = new LocalizationTablePageDto
            {
                LanguageCodes = languagesCode,
                Rows = new PagedResult<LocalizationTableRowDto>
                {
                    Items = rows,
                    Page = query.Page,
                    PageSize = query.PageSize,
                    Total = page.Total
                }
            };

            return Result.Ok(result);
        }

        private async Task<string[]> LoadLanguageCodesAsync(bool includeDisabledLanguages, CancellationToken ct)
        {
            var languages = await _languageRepository.GetAllAsync(includeDisabledLanguages, ct);
            
            return languages
                .OrderBy(l => l.Code, StringComparer.Ordinal)
                .Select(l => l.Code)
                .ToArray();
        }

        private static IReadOnlyList<LocalizationTableRowDto> BuildRows(
            IReadOnlyList<LocalizationKey> keys,
            IReadOnlyList<string> languageCodes)
        {
            return keys.Select(key => new LocalizationTableRowDto
            {
                KeyId = key.Id,
                Key = key.Key,
                ValuesByLanguage = languageCodes.ToDictionary(
                    code => code,
                    code => key.Translations
                        .FirstOrDefault(t => t.Language.Code == code)?.Value ?? string.Empty,
                    StringComparer.OrdinalIgnoreCase)
            }).ToList();
        }
    }
}
