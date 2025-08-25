using GameLocalization.Core.Domain.Entities.Identity;
using GameLocalization.Core.Interfaces.Repositories.Identity;
using GameLocalization.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameLocalization.Infrastructure.Repositories.Identity
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        { 
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct)
        {
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(x => x.Email == email, ct);
        }

        public async Task<User?> GetByEmailWithRoleAsync(string email, CancellationToken ct)
        {
            return await _context.Users
                .Include(x => x.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email, ct);
        }
        
        public async Task AddAsync(User user, CancellationToken ct)
        {
            await _context.Users.AddAsync(user, ct);
        }
    }
}
