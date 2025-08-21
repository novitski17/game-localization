using GameLocalization.Core.Domain.Entities;

namespace GameLocalization.Core.Interfaces.Repositories
{
    public interface ILanguageRepository
    {
        Task<IReadOnlyList<Language>> GetAllAsync(bool includeDisabled, CancellationToken ct);
        Task<Language?> FindByIdReadOnlyAsync(Guid id, CancellationToken ct);
        Task<Language?> FindByIdAsync(Guid id, CancellationToken ct);
        Task<bool> ExistsByCodeAsync(string code, CancellationToken ct);
        Task AddAsync(Language language, CancellationToken ct);
        void Remove(Language language);
    }
}
