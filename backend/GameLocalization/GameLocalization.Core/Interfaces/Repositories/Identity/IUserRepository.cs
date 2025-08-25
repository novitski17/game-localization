using GameLocalization.Core.Domain.Entities.Identity;

namespace GameLocalization.Core.Interfaces.Repositories.Identity
{
    public interface IUserRepository
    {
        Task<bool> ExistsByEmailAsync(string email, CancellationToken ct);
        Task<User?> GetByEmailWithRoleAsync(string email, CancellationToken ct);
        Task AddAsync(User user, CancellationToken ct);
    }
}
