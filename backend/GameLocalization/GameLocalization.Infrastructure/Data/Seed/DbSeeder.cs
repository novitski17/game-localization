using GameLocalization.Core.Domain.Constants;
using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.Domain.Entities.Identity;
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

        public static async Task SeedRolesAndAdminAsync(
            AppDbContext db,
            ILogger logger,
            string adminEmail,
            string adminPassword,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                logger.LogInformation("Admin seed skipped: email/password not provided.");
                return;
            }

            var adminRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == AppRoles.Admin, ct);
            if (adminRole == null)
            {
                adminRole = new Role { Id = Guid.NewGuid(), Name = AppRoles.Admin };
                await db.Roles.AddAsync(adminRole, ct);
                logger.LogInformation("Role '{Role}' created.", AppRoles.Admin);
            }

            var memberRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == AppRoles.Member, ct);
            if (memberRole == null)
            {
                memberRole = new Role { Id = Guid.NewGuid(), Name = AppRoles.Member };
                await db.Roles.AddAsync(memberRole, ct);
                logger.LogInformation("Role '{Role}' created.", AppRoles.Member);
            }

            var exists = await db.Users.AnyAsync(u => u.Email == adminEmail, ct);
            if (!exists)
            {
                var hash = BCrypt.Net.BCrypt.HashPassword(adminPassword);

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = adminEmail,
                    PasswordHash = hash,
                    RoleId = adminRole.Id,
                    Role = adminRole
                };
                await db.Users.AddAsync(user, ct);
                logger.LogInformation("Seed admin user created: {Email}", adminEmail);
            }


            await db.SaveChangesAsync(ct);
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
