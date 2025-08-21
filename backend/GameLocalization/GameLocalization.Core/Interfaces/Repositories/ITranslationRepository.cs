using GameLocalization.Core.Domain.Entities;

namespace GameLocalization.Core.Interfaces.Repositories
{
    public interface ITranslationRepository
    {
        Task<Translation?> FindByIdAsync(Guid id, CancellationToken ct);
    }
}
