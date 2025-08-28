using FluentResults;
using GameLocalization.Core.DTO.Translations;
using GameLocalization.Core.Interfaces.Services;
using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Core.Errors;
using FluentValidation;
using GameLocalization.Core.Validation;
using GameLocalization.Core.Exceptions;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace GameLocalization.Core.Services
{
    public class TranslationService : ITranslationService
    {
        private readonly ITranslationRepository _translationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateTranslationDto> _updateByIdValidator;
        private readonly IValidator<UpdateTranslationByKeyDto> _updateByKeyValidator;
        private readonly ILogger<TranslationService> _logger;

        public TranslationService(
            ITranslationRepository translationRepository,
            IUnitOfWork unitOfWork,
            IValidator<UpdateTranslationDto> updateByIdValidator,
            IValidator<UpdateTranslationByKeyDto> updateByKeyValidator,
            ILogger<TranslationService> logger,
            IMapper mapper)
        {
            _translationRepository = translationRepository ?? throw new ArgumentNullException(nameof(translationRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _updateByIdValidator = updateByIdValidator ?? throw new ArgumentNullException(nameof(updateByIdValidator));
            _updateByKeyValidator = updateByKeyValidator ?? throw new ArgumentNullException(nameof(updateByKeyValidator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result> UpdateAsync(Guid id, UpdateTranslationDto model, CancellationToken ct)
        {
            var validation = await _updateByIdValidator.ValidateAsync(model, ct);

            if (!validation.IsValid)
            {
                _logger.LogWarning("Validation failed when updating translation. Id={Id}", id);
                return Result.Fail(ValidationErrorsHelper.ToValidationAppError(validation));
            }

            var translation = await _translationRepository.FindByIdAsync(id, ct);

            if (translation is null)
            {
                _logger.LogWarning("Translation not found. Id={Id}", id);
                return Result.Fail(new NotFoundError("Translation", id.ToString()));
            }

            translation.Value = model.Value;

            try
            {
                await _unitOfWork.SaveChangesAsync(ct);
            }
            catch (DataAccessException ex)
            {
                _logger.LogError(ex,
                 "Database error while updating translation. Id={Id}",
                 id);
                return Result.Fail(new DatabaseError());
            }

            return Result.Ok();
        }

        public async Task<Result<TranslationDto>> UpdateByKeyAsync(
            UpdateTranslationByKeyDto model, 
            CancellationToken ct)
        {
            var validation = await _updateByKeyValidator.ValidateAsync(model, ct);

            if (!validation.IsValid)
            {
                _logger.LogWarning(
                    "Validation failed when updating translation by key. KeyId={KeyId}, Lang={Lang}",
                    model.LocalizationKeyId, model.LanguageCode);
                return Result.Fail(ValidationErrorsHelper.ToValidationAppError(validation));
            }

            var translation = await _translationRepository
                .FindByKeyAsync(model.LocalizationKeyId,
                    model.LanguageCode,
                    ct);

            if (translation is null)
            {
                return Result.Fail(new NotFoundError(
                    "Translation",
                    $"keyId={model.LocalizationKeyId}; lang={model.LanguageCode}"));
            }

            translation.Value = model.Value;

            try
            {
                await _unitOfWork.SaveChangesAsync(ct);
            }
            catch (DataAccessException ex)
            {
                _logger.LogError(ex,
                    "Database error while updating translation by key. KeyId={KeyId}, Lang={Lang}",
                    model.LocalizationKeyId, model.LanguageCode);
                return Result.Fail(new DatabaseError());
            }

            return Result.Ok(_mapper.Map<TranslationDto>(translation));
        }
    }
}
