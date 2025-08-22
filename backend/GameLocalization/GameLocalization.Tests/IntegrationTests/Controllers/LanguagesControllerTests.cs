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
    public class LanguagesControllerTests : ApiIntegrationTestBase
    {
        [Test]
        public async Task Get_Default_Should_Return_Only_Enabled()
        {
            await SeedAsync(db =>
            {
                db.Languages.AddRange(
                   new Language { Id = Guid.NewGuid(), Code = "en-us", Name = "English", IsEnabled = true },
                   new Language { Id = Guid.NewGuid(), Code = "pl", Name = "Polski", IsEnabled = false }
               );

                return Task.CompletedTask;
            });

            var response = await Client.GetAsync("api/v1/languages");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<List<LanguageDto>>();
            dto.Should().NotBeNull();
            dto!.Should().HaveCount(1);
            dto!.Select(x => x.Code).Should().BeEquivalentTo("en-us");
        }

        [Test]
        public async Task Get_With_Disabled_Should_Return_All()
        {
            await SeedAsync(db =>
            {
                db.Languages.AddRange(
                    new Language { Id = Guid.NewGuid(), Code = "en-us", Name = "English", IsEnabled = true },
                    new Language { Id = Guid.NewGuid(), Code = "pl", Name = "Polski", IsEnabled = false }
                );

                return Task.CompletedTask;
            });

            var response = await Client.GetAsync("api/v1/languages?includeDisabled=true");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<List<LanguageDto>>();
            dto.Should().NotBeNull();
            dto!.Should().HaveCount(2);
            dto!.Select(x => x.Code).Should().BeEquivalentTo("en-us", "pl");
        }

        [Test]
        public async Task Get_ById_Should_Return_404_When_NotFound()
        {
            var resp = await Client.GetAsync($"api/v1/languages/{Guid.NewGuid()}");
            resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Get_ById_Should_Return_200_And_Dto_When_Found()
        {
            var id = Guid.NewGuid();

            await SeedAsync(db =>
            {
                db.Languages.Add(new Language
                {
                    Id = id,
                    Code = "pl",
                    Name = "Polski",
                    IsEnabled = true
                });

                return Task.CompletedTask;
            });

            var resp = await Client.GetAsync($"api/v1/languages/{id}");
            resp.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await resp.Content.ReadFromJsonAsync<LanguageDto>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(id);
            dto.Code.Should().Be("pl");
            dto.Name.Should().Be("Polski");
            dto.IsEnabled.Should().BeTrue();
        }

        [Test]
        public async Task Patch_UpdateStatus_Should_Enable_And_Persist()
        {
            var id = Guid.NewGuid();

            await SeedAsync(db =>
            {
                db.Languages.Add(new Language
                {
                    Id = id,
                    Code = "en-us",
                    Name = "English",
                    IsEnabled = false
                });

                return Task.CompletedTask;
            });

            var patch = new HttpRequestMessage(HttpMethod.Patch, $"api/v1/languages/{id}/status")
            {
                Content = JsonContent.Create(new UpdateStatusRequest(true))
            };

            var resp = await Client.SendAsync(patch);
            resp.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await resp.Content.ReadFromJsonAsync<LanguageDto>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(id);
            dto.IsEnabled.Should().BeTrue();

            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var entity = await db.Languages.FindAsync(id);
            entity!.IsEnabled.Should().BeTrue();
        }

        private record LanguageDto(Guid Id, string Code, string Name, bool IsEnabled);
        private record UpdateStatusRequest(bool IsEnabled);

    }
}
