using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameLocalization.Infrastructure.Repositories
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly AppDbContext _context;
        public LanguageRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<Language?> FindByIdReadOnlyAsync(Guid id, CancellationToken ct)
        {
            return _context.Languages
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id, ct);
        }
        public Task<Language?> FindByIdAsync(Guid id, CancellationToken ct)
        {
            return _context.Languages
                .FirstOrDefaultAsync(l => l.Id == id, ct);
        }

        public async Task<IReadOnlyList<Language>> GetAllAsync(bool includeDisabled, CancellationToken ct)
        {
            return await _context.Languages
                .AsNoTracking()
                .Where(l => includeDisabled || l.IsEnabled)
                .ToListAsync(ct);
        }

        public async Task<bool> ExistsByCodeAsync(string code, CancellationToken ct)
        {
            var normalized = (code ?? string.Empty).Trim().ToLower();

            return await _context.Languages
                .AnyAsync(l => l.Code == normalized, ct);
        }

        public async Task AddAsync(Language language, CancellationToken ct)
        {
            await _context.Languages.AddAsync(language, ct);
        }

        public void Remove(Language language)
        {
            _context.Languages.Remove(language);
        }
    }
}