using FluentResults;
using GameLocalization.Core.DTO.Languages;

namespace GameLocalization.Core.Interfaces.Services
{
    public interface ILanguageService
    {
        Task<Result<IReadOnlyList<LanguageDto>>> GetAllAsync(bool includeDisabled, CancellationToken ct);
        Task<Result<LanguageDto>> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Result<LanguageDto>> CreateAsync(CreateLanguageDto model, CancellationToken ct);
        Task<Result<LanguageDto>> UpdateStatusAsync(Guid id, bool isEnabled, CancellationToken ct);
        Task<Result<LanguageDto>> UpdateAsync(Guid id, UpdateLanguageDto model, CancellationToken ct);
        Task<Result> DeleteAsync(Guid id, CancellationToken ct);
    }
}
