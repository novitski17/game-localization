using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Tests.TestSupport.Base;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace GameLocalization.Tests.IntegrationTests.Repositories
{
    [Category("Integration")]
    public class TranslationRepositoryTests : IntegrationTestBase
    {
        private ITranslationRepository TranslationRepository => Services.GetRequiredService<ITranslationRepository>();

        [Test]
        public async Task FindByIdAsync_Returns_Translation_When_Exists()
        {
            var lang = new Language { Id = Guid.NewGuid(), Code = "en-us", Name = "English", IsEnabled = true };
            var key = new LocalizationKey { Id = Guid.NewGuid(), Key = "menu.play" };
            var tr = new Translation { Id = Guid.NewGuid(), Language = lang, LocalizationKey = key, Value = "Play" };

            DbContext().Languages.Add(lang);
            DbContext().LocalizationKeys.Add(key);
            DbContext().Translations.Add(tr);
            await DbContext().SaveChangesAsync();

            var found = await TranslationRepository.FindByIdAsync(tr.Id, CancellationToken.None);

            found.Should().NotBeNull();
            found!.Value.Should().Be("Play");
            found.LanguageId.Should().Be(lang.Id);
            found.LocalizationKeyId.Should().Be(key.Id);
        }

        [Test]
        public async Task FindByIdAsync_Returns_Null_When_NotExists()
        {
            var found = await TranslationRepository.FindByIdAsync(Guid.NewGuid(), CancellationToken.None);
            found.Should().BeNull();
        }
    }
}
