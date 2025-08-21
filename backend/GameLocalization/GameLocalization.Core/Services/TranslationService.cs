using FluentResults;
using GameLocalization.Core.DTO.Translations;
using GameLocalization.Core.Interfaces.Services;
using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Core.Errors;
using FluentValidation;
using GameLocalization.Core.Validation;
using GameLocalization.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace GameLocalization.Core.Services
{
    public class TranslationService : ITranslationService
    {
        private readonly ITranslationRepository _translationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateTranslationDto> _updateValidator;
        private readonly ILogger<TranslationService> _logger;

        public TranslationService(
            ITranslationRepository translationRepository,
            IUnitOfWork unitOfWork,
            IValidator<UpdateTranslationDto> updateValidator,
            ILogger<TranslationService> logger)
        {
            _translationRepository = translationRepository ?? throw new ArgumentNullException(nameof(translationRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> UpdateAsync(Guid id, UpdateTranslationDto model, CancellationToken ct)
        {
            var validation = await _updateValidator.ValidateAsync(model, ct);

            if (!validation.IsValid)
            {
                _logger.LogWarning("Validation failed when updating translation. Id={Id}", id);
                return Result.Fail(ValidationErrorsHelper.ToValidationAppError(validation));
            }

            var entity = await _translationRepository.FindByIdAsync(id, ct);

            if (entity is null)
            {
                _logger.LogWarning("Translation not found. Id={Id}", id);
                return Result.Fail(new NotFoundError("Translation", id.ToString()));
            }

            entity.Value = model.Value;

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
    }
}
