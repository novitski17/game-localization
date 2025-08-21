using GameLocalization.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameLocalization.Infrastructure.Data.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db, ILogger logger, CancellationToken ct = default)
        {
            await SeedLanguagesAsync(db, logger, ct);
            await SeedKeysAsync(db, logger, ct);
            await SeedTranslationsAsync(db, logger, ct);
        }

        private static async Task SeedLanguagesAsync(AppDbContext db, ILogger logger, CancellationToken ct)
        {
            if (await db.Languages.AnyAsync(ct))
                return;

            var languages = new[]
            {
                new Language { Code = "ru", Name = "Русский",  IsEnabled = true  },
                new Language { Code = "en", Name = "English",  IsEnabled = true  },
                new Language { Code = "it", Name = "Italiano", IsEnabled = true  },
                new Language { Code = "de", Name = "Deutsch",  IsEnabled = false },
            };

            await db.Languages.AddRangeAsync(languages, ct);
            await db.SaveChangesAsync(ct);

            logger.LogInformation("Seeded {Count} languages: {Codes}",
                languages.Length, string.Join(", ", languages.Select(l => l.Code)));
        }

        private static async Task SeedKeysAsync(AppDbContext db, ILogger logger, CancellationToken ct)
        {
            if (await db.LocalizationKeys.AnyAsync(ct))
                return;

            var keyEntities = SeedData.Keys
                .Select(k => new LocalizationKey { Key = k })
                .ToList();

            await db.LocalizationKeys.AddRangeAsync(keyEntities, ct);
            await db.SaveChangesAsync(ct);

            logger.LogInformation("Seeded {Count} localization keys.", keyEntities.Count);
        }

        private static async Task SeedTranslationsAsync(AppDbContext db, ILogger logger, CancellationToken ct)
        {
            if (await db.Translations.AnyAsync(ct))
                return;

            var langs = await db.Languages.AsNoTracking().ToListAsync(ct);
            var keys = await db.LocalizationKeys.AsNoTracking().ToListAsync(ct);

            var translations = keys
                .SelectMany(key => langs.Select(lang => new Translation
                {
                    LocalizationKeyId = key.Id,
                    LanguageId = lang.Id,
                    Value = lang.IsEnabled
                        ? SeedData.GetValue(lang.Code, key.Key)
                        : string.Empty
                }))
                .ToList();

            await db.Translations.AddRangeAsync(translations, ct);
            await db.SaveChangesAsync(ct);

            logger.LogInformation("Seeded translations: {Pairs} (keys × langs).",
                keys.Count * langs.Count);
        }
    }
}
