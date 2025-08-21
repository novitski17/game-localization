using GameLocalization.Core.Exceptions;
using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Infrastructure.Data;

namespace GameLocalization.Infrastructure.Repositories
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public EfUnitOfWork(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task SaveChangesAsync(CancellationToken ct)
        {
            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Database operation failed.", ex);
            }
        }
    }
}