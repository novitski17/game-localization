using GameLocalization.Infrastructure.Data;
using GameLocalization.Tests.TestSupport.Base;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GameLocalization.Core.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using GameLocalization.Tests.TestSupport.Auth;

namespace GameLocalization.Tests.IntegrationTests.Controllers
{
    [Category("Integration")]
    public class LocalizationKeysControllerTests : ApiIntegrationTestBase
    {
        [Test]
        public async Task Get_ById_Should_Return_404_When_NotFound()
        {
            Client.AsMember();
            var resp = await Client.GetAsync($"api/v1/localization-keys/{Guid.NewGuid()}");
            resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Post_Create_Should_Return_201_And_Normalized_Row_With_Empty_Values()
        {
            await SeedAsync(db =>
            {
                db.Languages.AddRange(
                    new Language { Id = Guid.NewGuid(), Code = "en-us", Name = "English", IsEnabled = true },
                    new Language { Id = Guid.NewGuid(), Code = "pl", Name = "Polski", IsEnabled = true }
                );
                return Task.CompletedTask;
            });

            var cmd = new CreateKeyRequest("Menu.Play ");
            Client.AsMember();

            var resp = await Client.PostAsJsonAsync("api/v1/localization-keys", cmd);
            resp.StatusCode.Should().Be(HttpStatusCode.Created);

            var row = await resp.Content.ReadFromJsonAsync<KeyRowDto>();

            row.Should().NotBeNull();
            row!.Key.Should().Be("menu.play");
            row.ValuesByLanguage.Should().ContainKeys("en-us", "pl");
            row.ValuesByLanguage["en-us"].Should().BeEmpty();
            row.ValuesByLanguage["pl"].Should().BeEmpty();

            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.LocalizationKeys.Count().Should().Be(1);
        }

        [Test]
        public async Task Delete_Should_Remove_And_Return_200()
        {
            Guid id = Guid.NewGuid();

            await SeedAsync(db =>
            {
                db.LocalizationKeys.Add(new LocalizationKey { Id = id, Key = "menu.exit" });
                return Task.CompletedTask;
            });

            Client.AsMember();

            var resp = await Client.DeleteAsync($"api/v1/localization-keys/{id}");
            resp.StatusCode.Should().Be(HttpStatusCode.NoContent);

            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            (await db.LocalizationKeys.FindAsync(id)).Should().BeNull();
        }

        private record CreateKeyRequest(string Key);
        private record KeyRowDto(Guid KeyId, string Key, Dictionary<string, string> ValuesByLanguage);
    }
}
