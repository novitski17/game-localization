using GameLocalization.Infrastructure.Data;
using GameLocalization.Tests.TestSupport.Base;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GameLocalization.Core.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace GameLocalization.Tests.IntegrationTests.Controllers
{
    [Category("Integration")]
    public class TranslationsControllerTests : ApiIntegrationTestBase
    {
        [Test]
        public async Task Patch_Update_Should_Change_Value_And_Persist()
        {
            Guid id = Guid.NewGuid();
            Guid en = Guid.NewGuid();

            await SeedAsync(db =>
            {
                db.Languages.Add(new Language { Id = en, Code = "en-us", Name = "English", IsEnabled = true });
                db.LocalizationKeys.Add(new LocalizationKey
                {
                    Id = Guid.NewGuid(),
                    Key = "menu.play",
                    Translations = new List<Translation>
                    {
                        new() { Id = id, LanguageId = en, Value = "Old" }
                    }
                });
                return Task.CompletedTask;
            });

            var resp = await Client
                .PatchAsJsonAsync($"api/v1/translations/{id}", new UpdateTranslationRequest("New Value"));
            resp.StatusCode.Should().Be(HttpStatusCode.NoContent);

            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var entity = await db.Translations.FindAsync(id);
            entity!.Value.Should().Be("New Value");
        }

        [Test]
        public async Task Patch_Update_Should_Return_404_When_NotFound()
        {
            var resp = await Client.PatchAsJsonAsync($"api/v1/translations/{Guid.NewGuid()}",
                                                     new UpdateTranslationRequest("x"));
            resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private record UpdateTranslationRequest(string Value);
    }
}
