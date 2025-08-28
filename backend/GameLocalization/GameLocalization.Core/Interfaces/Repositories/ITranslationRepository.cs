using GameLocalization.Core.Domain.Entities;

namespace GameLocalization.Core.Interfaces.Repositories
{
    public interface ITranslationRepository
    {
        Task<Translation?> FindByIdAsync(Guid id, CancellationToken ct);
        Task<Translation?> FindByKeyAsync(Guid localizationKeyId,
            string languageCode,
            CancellationToken ct);
    }
}
