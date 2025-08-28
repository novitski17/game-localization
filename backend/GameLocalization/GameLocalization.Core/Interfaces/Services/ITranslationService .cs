using FluentResults;
using GameLocalization.Core.DTO.Translations;

namespace GameLocalization.Core.Interfaces.Services
{
    public interface ITranslationService
    {
        Task<Result> UpdateAsync(Guid id, UpdateTranslationDto model, CancellationToken ct);
        Task<Result<TranslationDto>> UpdateByKeyAsync(UpdateTranslationByKeyDto model, CancellationToken ct);
    }
}
