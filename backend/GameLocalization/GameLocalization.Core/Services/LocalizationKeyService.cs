using FluentResults;
using FluentValidation;
using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.DTO.Common;
using GameLocalization.Core.DTO.Keys;
using GameLocalization.Core.Errors;
using GameLocalization.Core.Exceptions;
using GameLocalization.Core.Extensions;
using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Core.Interfaces.Services;
using GameLocalization.Core.Validation;
using Microsoft.Extensions.Logging;

namespace GameLocalization.Core.Services
{
    public class LocalizationKeyService : ILocalizationKeyService
    {
        private readonly ILocalizationKeyRepository _keyRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateLocalizationKeyDto> _createValidator;
        private readonly ILogger<LocalizationKeyService> _logger;

        public LocalizationKeyService(
            ILocalizationKeyRepository keyRepository,
            ILanguageRepository languageRepository,
            IUnitOfWork unitOfWork,
            IValidator<CreateLocalizationKeyDto> createValidator,
            ILogger<LocalizationKeyService> logger)
        {
            _keyRepository = keyRepository ?? throw new ArgumentNullException(nameof(keyRepository));
            _languageRepository = languageRepository ?? throw new ArgumentNullException(nameof(languageRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<Result<LocalizationTableRowDto>> GetByIdAsync(Guid id, bool includeDisabledLanguages, CancellationToken ct)
        {
            var key = await _keyRepository.FindByIdReadOnlyWithTranslationsAsync(id, ct);

            if (key == null)
            {
                _logger.LogWarning("Localization key not found. Id={Id}", id);
                return Result.Fail(new NotFoundError("LocalizationKey", id));
            }

            var languages = await _languageRepository.GetAllAsync(includeDisabledLanguages, ct);

            var row = ToTableRowDto(key, languages);

            return Result.Ok(row);
        }

        public async Task<Result<LocalizationTableRowDto>> CreateAsync(CreateLocalizationKeyDto model, CancellationToken ct)
        {
            var validation = await _createValidator.ValidateAsync(model, ct);

            if (!validation.IsValid)
            {
                return Result.Fail(ValidationErrorsHelper.ToValidationAppError(validation));
            }

            var normalizedKey = model.Key.NormalizeCode();

            if (await _keyRepository.ExistsByKeyAsync(normalizedKey, ct))
            {
                return Result.Fail(new ConflictError($"Localization key '{normalizedKey}' already exists."));
            }

            var allLanguages = await _languageRepository.GetAllAsync(includeDisabled: true, ct);

            var entity = new LocalizationKey
            {
                Key = normalizedKey,
                Translations = allLanguages.Select(l => new Translation
                {
                    LanguageId = l.Id,
                    Value = string.Empty
                }).ToList()
            };

            try
            {
                await _keyRepository.AddAsync(entity, ct);
                await _unitOfWork.SaveChangesAsync(ct);
            }
            catch (DataAccessException ex)
            {
                _logger.LogError(ex,
                    "Database error while creating localization key. Key={Key}",
                    normalizedKey);
                return Result.Fail(new DatabaseError());
            }

            var row = ToTableRowDto(entity, allLanguages);
            return Result.Ok(row);
        }

        public async Task<Result> DeleteAsync(Guid id, CancellationToken ct)
        {
            var key = await _keyRepository.FindByIdAsync(id, ct);

            if (key == null)
            {
                _logger.LogWarning("Localization key with ID {KeyId} not found", id);
                return Result.Fail(new NotFoundError("LocalizationKey", id));
            }

            _keyRepository.Remove(key);

            try
            {
                await _unitOfWork.SaveChangesAsync(ct);
            }
            catch (DataAccessException ex)
            {
                _logger.LogError(ex, "Database error while deleting localization key. Key={KeyId}", id);
                return Result.Fail(new DatabaseError());
            }

            return Result.Ok();
        }

        private static LocalizationTableRowDto ToTableRowDto(
            LocalizationKey key,
            IEnumerable<Language> languages)
        {
            var translations = languages.ToDictionary(
                l => l.Code,
                l =>
                {
                    var t = key.Translations?.FirstOrDefault(x => x.LanguageId == l.Id);
                    return t?.Value ?? string.Empty;
                });

            return new LocalizationTableRowDto
            {
                KeyId = key.Id,
                Key = key.Key,
                ValuesByLanguage = translations
            };
        }
    }
}
