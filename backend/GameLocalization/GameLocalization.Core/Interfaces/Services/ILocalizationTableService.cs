using FluentResults;
using GameLocalization.Core.DTO.Table;

namespace GameLocalization.Core.Interfaces.Services
{
    public interface ILocalizationTableService
    {
        Task<Result<LocalizationTablePageDto>> GetAsync(
            LocalizationTableQueryDto query,
            CancellationToken ct);
    }
}
