using FluentResults;
using GameLocalization.Core.DTO.Common;
using GameLocalization.Core.DTO.Keys;

namespace GameLocalization.Core.Interfaces.Services
{
    public interface ILocalizationKeyService
    {
        Task<Result<LocalizationTableRowDto>> GetByIdAsync(Guid id, bool includeDisabledLanguages, CancellationToken ct);
        Task<Result<LocalizationTableRowDto>> CreateAsync(CreateLocalizationKeyDto model, CancellationToken ct);
        Task<Result> DeleteAsync(Guid id, CancellationToken ct);
    }
}