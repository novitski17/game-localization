using GameLocalization.Core.Domain.Entities.Identity;
using GameLocalization.Core.Interfaces.Repositories.Identity;
using GameLocalization.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameLocalization.Infrastructure.Repositories.Identity
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Role?> GetByNameAsync(string name, CancellationToken ct)
        {
            return await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name == name, ct);
        }

        public async Task AddAsync(Role role, CancellationToken ct)
        {
            await _context.Roles.AddAsync(role, ct);
        }
    }
}
