using FluentAssertions;
using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Tests.TestSupport.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GameLocalization.Tests.IntegrationTests.Repositories
{
    [Category("Integration")]
    public class LanguageRepositoryTests : IntegrationTestBase
    {
        private ILanguageRepository LanguageRepository => Services.GetRequiredService<ILanguageRepository>();

        [Test]
        public async Task Add_And_GetAll_With_Filter_Works()
        {
            var db = DbContext();
            db.Languages.AddRange(
                new Language { Id = Guid.NewGuid(), Code = "en-us", Name = "English", IsEnabled = true },
                new Language { Id = Guid.NewGuid(), Code = "pl", Name = "Polski", IsEnabled = false }
            );
            await db.SaveChangesAsync();

            var onlyEnabled = await LanguageRepository.GetAllAsync(includeDisabled: false, CancellationToken.None);
            onlyEnabled.Should().ContainSingle(x => x.Code == "en-us");

            var all = await LanguageRepository.GetAllAsync(includeDisabled: true, CancellationToken.None);
            all.Should().HaveCount(2);
        }

        [Test]
        public async Task ExistsByCode_Is_Normalized_And_Unique()
        {
            var db = DbContext();
            db.Languages.Add(new Language { Id = Guid.NewGuid(), Code = "en-us", Name = "English", IsEnabled = true });
            await db.SaveChangesAsync();

            (await LanguageRepository.ExistsByCodeAsync(" En-US ", CancellationToken.None)).Should().BeTrue();
            (await LanguageRepository.ExistsByCodeAsync("de", CancellationToken.None)).Should().BeFalse();

            db.Languages.Add(new Language { Id = Guid.NewGuid(), Code = "en-us", Name = "Dup", IsEnabled = true });
            Func<Task> act = async () => await db.SaveChangesAsync();
            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Test]
        public async Task FindById_ReadOnly_And_Tracked()
        {
            var id = Guid.NewGuid();
            var db = DbContext();
            db.Languages.Add(new Language { Id = id, Code = "pl", Name = "Polski", IsEnabled = true });
            await db.SaveChangesAsync();

            var ro = await LanguageRepository.FindByIdReadOnlyAsync(id, CancellationToken.None);
            ro!.Name.Should().Be("Polski");

            var tracked = await LanguageRepository.FindByIdAsync(id, CancellationToken.None);
            tracked!.IsEnabled = false;
            await DbContext().SaveChangesAsync();

            (await LanguageRepository.FindByIdReadOnlyAsync(id, CancellationToken.None))!.IsEnabled.Should().BeFalse();
        }

        [Test]
        public async Task Remove_Works()
        {
            var id = Guid.NewGuid();
            var db = DbContext();
            var lang = new Language { Id = id, Code = "fr", Name = "Français", IsEnabled = true };
            db.Languages.Add(lang);
            await db.SaveChangesAsync();

            LanguageRepository.Remove(lang);
            await DbContext().SaveChangesAsync();

            (await LanguageRepository.FindByIdReadOnlyAsync(id, CancellationToken.None)).Should().BeNull();
        }
    }
}