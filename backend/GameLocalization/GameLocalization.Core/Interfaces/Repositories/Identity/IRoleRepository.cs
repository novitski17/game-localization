using GameLocalization.Core.Domain.Entities.Identity;

namespace GameLocalization.Core.Interfaces.Repositories.Identity
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string name, CancellationToken ct);
        Task AddAsync(Role role, CancellationToken ct);
    }
}
