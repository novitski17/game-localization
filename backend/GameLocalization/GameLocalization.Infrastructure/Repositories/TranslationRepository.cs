using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameLocalization.Infrastructure.Repositories
{
    public class TranslationRepository : ITranslationRepository
    {
        private readonly AppDbContext _context;

        public TranslationRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Translation?> FindByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Translations
                .FirstOrDefaultAsync(t => t.Id == id, ct);
        }

        public async Task<Translation?> FindByKeyAsync(
            Guid localizationKeyId,
            string languageCode,
            CancellationToken ct)
        {
            var normalizedCode = (languageCode ?? string.Empty).Trim().ToLower();

            return await _context.Translations
                .Include(t => t.Language)
                .FirstOrDefaultAsync(t => t.Language.Code == normalizedCode
                    && t.LocalizationKeyId == localizationKeyId, ct);
        }
    }
}
