using AutoMapper;
using FluentResults;
using FluentValidation;
using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.DTO.Languages;
using GameLocalization.Core.Errors;
using GameLocalization.Core.Exceptions;
using GameLocalization.Core.Extensions;
using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Core.Interfaces.Services;
using GameLocalization.Core.Validation;
using Microsoft.Extensions.Logging;

namespace GameLocalization.Core.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<LanguageService> _logger;
        private readonly IValidator<CreateLanguageDto> _createValidator;
        private readonly IValidator<UpdateLanguageDto> _updateValidator;

        public LanguageService(ILanguageRepository languageRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CreateLanguageDto> createValidator,
            IValidator<UpdateLanguageDto> updateValidator,
            ILogger<LanguageService> logger)
        {
            _languageRepository = languageRepository ?? throw new ArgumentNullException(nameof(languageRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        public async Task<Result<LanguageDto>> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var language = await _languageRepository.FindByIdAsync(id, ct);

            if (language == null)
            {
                return Result.Fail(new NotFoundError("Language", id));
            }

            return Result.Ok(_mapper.Map<LanguageDto>(language));
        }

        public async Task<Result<IReadOnlyList<LanguageDto>>> GetAllAsync(bool includeDisabled, CancellationToken ct)
        {
            var languages = await _languageRepository.GetAllAsync(includeDisabled, ct);

            var dtos = _mapper.Map<IReadOnlyList<LanguageDto>>(languages);

            return Result.Ok(dtos); 
        }

        public async Task<Result<LanguageDto>> CreateAsync(CreateLanguageDto model, CancellationToken ct)
        {
            var validation = await _createValidator.ValidateAsync(model, ct);

            if (!validation.IsValid)
            {
                return Result.Fail(ValidationErrorsHelper.ToValidationAppError(validation));
            }

            var normalizedCode = model.Code.NormalizeCode();

            if (await _languageRepository.ExistsByCodeAsync(normalizedCode, ct))
            {
                return Result.Fail(new ConflictError($"Language with code '{normalizedCode}' already exists."));
            }

            var language = _mapper.Map<Language>(model);

            try
            {
                await _languageRepository.AddAsync(language, ct);
                await _unitOfWork.SaveChangesAsync(ct);
            }
            catch (DataAccessException ex)
            {
                _logger.LogError(ex,
                    "Database error while creating language with code {Code}",
                    language.Code);
                return Result.Fail(new DatabaseError());
            }

            return Result.Ok(_mapper.Map<LanguageDto>(language));
        }

        public async Task<Result<LanguageDto>> UpdateStatusAsync(Guid id, bool isEnabled, CancellationToken ct)
        {
            var language = await _languageRepository.FindByIdAsync(id, ct);

            if (language == null)
            {
                return Result.Fail(new NotFoundError("Language", id)); 
            }

            language.IsEnabled = isEnabled;

            try 
            {
                await _unitOfWork.SaveChangesAsync(ct);
            }
            catch (DataAccessException ex)
            {
                _logger.LogError(ex,
                    "Database error while updating language status. Id={Id}, Code={Code}",
                    id, language.Code);
                return Result.Fail(new DatabaseError());
            }

            return Result.Ok(_mapper.Map<LanguageDto>(language));
        }

        public async Task<Result<LanguageDto>> UpdateAsync(Guid id, UpdateLanguageDto model, CancellationToken ct)
        {
            var validation = await _updateValidator.ValidateAsync(model, ct);

            if (!validation.IsValid)
            {
                return Result.Fail(ValidationErrorsHelper.ToValidationAppError(validation));
            }

            var language = await _languageRepository.FindByIdAsync(id, ct);

            if (language == null)
            {
                return Result.Fail(new NotFoundError("Language", id));
            }

            var normalizedCode = model.Code.NormalizeCode();

            if (!normalizedCode.Equals(language.Code, StringComparison.OrdinalIgnoreCase))
            {
                var exists = await _languageRepository.ExistsByCodeAsync(normalizedCode, ct);
                if (exists)
                    return Result.Fail(new ConflictError($"Language with code '{normalizedCode}' already exists."));
            }

            language.Name = model.Name;
            language.Code = model.Code.NormalizeCode();
            language.IsEnabled = model.IsEnabled;

            try
            {
                await _unitOfWork.SaveChangesAsync(ct);
            }
            catch (DataAccessException ex)
            {
                _logger.LogError(ex,
                 "Database error while updating language. Id={Id}, Code={Code}",
                 id, language.Code);
                return Result.Fail(new DatabaseError());
            }

            return Result.Ok(_mapper.Map<LanguageDto>(language));
        }

        public async Task<Result> DeleteAsync(Guid id, CancellationToken ct)
        {
            var language =  await _languageRepository.FindByIdAsync(id, ct);
            if (language == null)
            {
                return Result.Fail(new NotFoundError("Language", id));
            }

            _languageRepository.Remove(language);

            try
            {
                await _unitOfWork.SaveChangesAsync(ct);
            }
            catch (DataAccessException ex)
            {
                _logger.LogError(ex,
                 "Database error while deleting language. Id={Id}, Code={Code}",
                 id, language.Code);
                return Result.Fail(new DatabaseError());
            }

            return Result.Ok();
        }

    }
}