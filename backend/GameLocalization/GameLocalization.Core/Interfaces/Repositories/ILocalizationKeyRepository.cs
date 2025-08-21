using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.DTO.Common;

namespace GameLocalization.Core.Interfaces.Repositories
{
    public interface ILocalizationKeyRepository
    {
        Task<LocalizationKey?> FindByIdAsync(Guid id, CancellationToken ct);
        Task<LocalizationKey?> FindByIdReadOnlyWithTranslationsAsync(Guid id, CancellationToken ct);
        Task<bool> ExistsByKeyAsync(string key, CancellationToken ct);
        Task AddAsync(LocalizationKey key, CancellationToken ct);
        void Remove(LocalizationKey key);

        Task<PagedSlice<LocalizationKey>> GetPageWithTranslationsAsync(
            int page,
            int pageSize,
            string? search,
            bool includeDisabled,
            CancellationToken ct);
    }
}
