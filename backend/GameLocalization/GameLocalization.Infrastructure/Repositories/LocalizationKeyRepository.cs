using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.DTO.Common;
using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameLocalization.Infrastructure.Repositories
{
    public class LocalizationKeyRepository : ILocalizationKeyRepository
    {
        private readonly AppDbContext _context;

        public LocalizationKeyRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<LocalizationKey?> FindByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.LocalizationKeys
                .Include(k => k.Translations)
                .FirstOrDefaultAsync(k => k.Id == id, ct);
        }

        public async Task<LocalizationKey?> FindByIdReadOnlyWithTranslationsAsync(Guid id, CancellationToken ct)
        {
            return await _context.LocalizationKeys
                .AsNoTracking()
                .Include(k => k.Translations)
                .FirstOrDefaultAsync(k => k.Id == id, ct);
        }

        public Task<bool> ExistsByKeyAsync(string key, CancellationToken ct)
        {
            var normalized = (key ?? string.Empty).Trim();

            return _context.LocalizationKeys.AnyAsync(k => k.Key == normalized, ct);
        }

        public async Task AddAsync(LocalizationKey key, CancellationToken ct)
        {
            await _context.LocalizationKeys.AddAsync(key, ct);
        }


        public void Remove(LocalizationKey key)
        {
            _context.LocalizationKeys.Remove(key);
        }

        public async Task<PagedSlice<LocalizationKey>> GetPageWithTranslationsAsync(
            int page,
            int pageSize,
            string? search,
            bool includeDisabled,
            CancellationToken ct)
        {
            var query = BuildBaseQuery(includeDisabled);

            query = ApplySearchByKeyOnly(query, search);

            var total = await query.LongCountAsync(ct);
            var items = await ApplyPaging(query, page, pageSize).
                ToListAsync(ct);

            return new PagedSlice<LocalizationKey>
            {
                Items = items,
                Total = total,
            };
        }

        private IQueryable<LocalizationKey> BuildBaseQuery(bool includeDisabled)
        {
            return _context.LocalizationKeys
                .AsNoTracking()
                .Include(k => k.Translations
                    .Where(t => includeDisabled || ( t.Language != null && t.Language.IsEnabled)) )
                .ThenInclude(t => t.Language);
        }

        private static IQueryable<LocalizationKey> ApplySearchByKeyOnly(
            IQueryable<LocalizationKey> query,
            string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return query;
            }

            return query.Where(k => k.Key.Contains(search));
        }

        private static IQueryable<LocalizationKey> ApplyPaging(
            IQueryable<LocalizationKey> query,
            int page,
            int pageSize)
        {
            return query
                .OrderBy(k => k.Key)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }
    }
}
