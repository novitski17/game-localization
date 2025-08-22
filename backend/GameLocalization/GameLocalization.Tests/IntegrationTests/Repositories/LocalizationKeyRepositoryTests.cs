using FluentAssertions;
using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Tests.TestSupport.Base;
using Microsoft.Extensions.DependencyInjection;

namespace GameLocalization.Tests.IntegrationTests.Repositories
{
    [Category("Integration")]
    public class LocalizationKeyRepositoryTests : IntegrationTestBase
    {
        private ILocalizationKeyRepository LocalizationKeyRepository => Services.GetRequiredService<ILocalizationKeyRepository>();

        private (Language en, Language pl) SeedLanguages()
        {
            var en = new Language { Id = Guid.NewGuid(), Code = "en-us", Name = "English", IsEnabled = true };
            var pl = new Language { Id = Guid.NewGuid(), Code = "pl", Name = "Polski", IsEnabled = true };
            DbContext().Languages.AddRange(en, pl);
            DbContext().SaveChanges();
            return (en, pl);
        }

        [Test]
        public async Task ExistsByKey_Normalizes_And_Respects_Unique_Index()
        {
            DbContext().LocalizationKeys.Add(new LocalizationKey { Id = Guid.NewGuid(), Key = "menu.play" });
            await DbContext().SaveChangesAsync();

            (await LocalizationKeyRepository.ExistsByKeyAsync(" menu.play ", CancellationToken.None)).Should().BeTrue();
            (await LocalizationKeyRepository.ExistsByKeyAsync("menu.exit", CancellationToken.None)).Should().BeFalse();

            DbContext().LocalizationKeys.Add(new LocalizationKey { Id = Guid.NewGuid(), Key = "menu.play" });
            await FluentActions.Awaiting(() => DbContext().SaveChangesAsync())
                .Should().ThrowAsync<Microsoft.EntityFrameworkCore.DbUpdateException>();
        }

        [Test]
        public async Task FindById_ReadOnly_With_Translations_Includes_Language()
        {
            var (en, pl) = SeedLanguages();
            var key = new LocalizationKey
            {
                Id = Guid.NewGuid(),
                Key = "menu.exit",
                Translations = new List<Translation>
                {
                    new() { Id = Guid.NewGuid(), LanguageId = en.Id, Value = "Exit" },
                    new() { Id = Guid.NewGuid(), LanguageId = pl.Id, Value = "Wyjście" }
                }
            };
            DbContext().LocalizationKeys.Add(key);
            await DbContext().SaveChangesAsync();

            var reloaded = await LocalizationKeyRepository.FindByIdReadOnlyWithTranslationsAsync(key.Id, CancellationToken.None);
            reloaded!.Translations.Should().HaveCount(2);
            reloaded.Translations.Should().OnlyContain(t => t.LanguageId != Guid.Empty);
        }

        [Test]
        public async Task GetPageWithTranslations_Applies_Search_Filter_Paging_And_IncludeDisabled()
        {
            var (en, pl) = SeedLanguages();

            pl.IsEnabled = false;
            await DbContext().SaveChangesAsync();

            var keys = new[]
            {
                new LocalizationKey
                {
                    Id = Guid.NewGuid(), Key = "menu.play",
                    Translations = new List<Translation>
                    {
                        new() { Id = Guid.NewGuid(), LanguageId = en.Id, Value = "Play" },
                        new() { Id = Guid.NewGuid(), LanguageId = pl.Id, Value = "Graj" }
                    }
                },
                new LocalizationKey
                {
                    Id = Guid.NewGuid(), Key = "menu.exit",
                    Translations = new List<Translation>
                    {
                        new() { Id = Guid.NewGuid(), LanguageId = en.Id, Value = "Exit" }
                    }
                },
                new LocalizationKey { Id = Guid.NewGuid(), Key = "settings.audio" }
            };
            DbContext().LocalizationKeys.AddRange(keys);
            await DbContext().SaveChangesAsync();

            var page = await LocalizationKeyRepository.GetPageWithTranslationsAsync(1, 10, "menu", includeDisabled: false,
                CancellationToken.None);

            page.Total.Should().Be(2);
            page.Items.Should().HaveCount(2);
            page.Items.Select(i => i.Key).Should().BeInAscendingOrder(); 

            var play = page.Items.First(k => k.Key == "menu.play");
            play.Translations.Should().OnlyContain(t => t.Language != null && t.Language.IsEnabled);

            var exit = page.Items.First(k => k.Key == "menu.exit");
            exit.Translations.Should().OnlyContain(t => t.Language != null && t.Language.IsEnabled);

            var page1 = await LocalizationKeyRepository.GetPageWithTranslationsAsync(1, 1, "menu", false, CancellationToken.None);
            var page2 = await LocalizationKeyRepository.GetPageWithTranslationsAsync(2, 1, "menu", false, CancellationToken.None);

            page1.Items.Should().HaveCount(1);
            page2.Items.Should().HaveCount(1);
            page1.Items[0].Key.Should().NotBe(page2.Items[0].Key);
        }

        [Test]
        public async Task Add_And_Remove_Works()
        {
            var key = new LocalizationKey { Id = Guid.NewGuid(), Key = "menu.about" };
            await LocalizationKeyRepository.AddAsync(key, CancellationToken.None);
            await DbContext().SaveChangesAsync();

            (await LocalizationKeyRepository.FindByIdAsync(key.Id, CancellationToken.None))!.Key.Should().Be("menu.about");

            LocalizationKeyRepository.Remove(key);
            await DbContext().SaveChangesAsync();

            (await LocalizationKeyRepository.FindByIdAsync(key.Id, CancellationToken.None)).Should().BeNull();
        }
    }
}
