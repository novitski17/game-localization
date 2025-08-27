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

            Client.AsMember();

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

            Client.AsMember();

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
            Client.AsMember();
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

            Client.AsMember();

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

            Client.AsMember();

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

        [Test]
        public async Task Get_Languages_WithoutAuth_Should_Return_401()
        {
            Client.ClearAuth();

            var resp = await Client.GetAsync("api/v1/languages");

            resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Post_Admin_Should_Create_And_Return_201()
        {
            Client.AsAdmin();
            var body = new { Code = "it", Name = "Italian", IsEnabled = true };

            var resp = await Client.PostAsJsonAsync("/api/v1/languages", body);

            resp.StatusCode.Should().Be(HttpStatusCode.Created);
            var dto = await resp.Content.ReadFromJsonAsync<LanguageDto>();
            dto.Should().NotBeNull();
            dto!.Code.Should().Be("it");
            dto.Name.Should().Be("Italian");
            dto.IsEnabled.Should().BeTrue();
        }


        [Test]
        public async Task Post_Admin_Should_Return_409_On_Duplicate_Code()
        {
            var id = Guid.NewGuid();

            await SeedAsync(db =>
            {
                db.Languages.Add(new Language
                {
                    Id = id,
                    Code = "de",
                    Name = "Deutsch",
                    IsEnabled = true
                });

                return Task.CompletedTask;
            });

            Client.AsAdmin();

            var dup = await Client
                .PostAsJsonAsync("/api/v1/languages", new { Code = "de", Name = "Deutsch", IsEnabled = true });


            dup.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Test]
        public async Task Put_Admin_Should_Update_And_Return_200()
        {
            var id = Guid.NewGuid();

            await SeedAsync(db =>
            {
                db.Languages.Add(new Language
                {
                    Id = id,
                    Code = "es",
                    Name = "Spanish",
                    IsEnabled = true
                });

                return Task.CompletedTask;
            });

            Client.AsAdmin();

            var updateBody = new
            {
                Code = "es-es",
                Name = "Español",
                IsEnabled = false
            };

            var resp = await Client.PutAsJsonAsync($"/api/v1/languages/{id}", updateBody);

            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            var dto = await resp.Content.ReadFromJsonAsync<LanguageDto>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(id);
            dto.Code.Should().Be("es-es");
            dto.Name.Should().Be("Español");
            dto.IsEnabled.Should().BeFalse();
        }

        [Test]
        public async Task Put_Admin_Should_Return_404_When_NotFound()
        {
            Client.AsAdmin();
            var updateBody = new { Code = "no", Name = "Norsk", IsEnabled = true };

            var resp = await Client.PutAsJsonAsync($"/api/v1/languages/{Guid.NewGuid()}", updateBody);

            resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Delete_Admin_Should_Return_204_And_Then_404_On_Get()
        {
            Client.AsAdmin();
            var create = await Client.PostAsJsonAsync("/api/v1/languages",
                new
                {
                    Code = "cz",
                    Name = "Czech",
                    IsEnabled = true,
                });

            var created = await create.Content.ReadFromJsonAsync<LanguageDto>();

            var del = await Client.DeleteAsync($"/api/v1/languages/{created!.Id}");

            del.StatusCode.Should().Be(HttpStatusCode.NoContent);

            Client.AsMember();
            var getAfter = await Client.GetAsync($"/api/v1/languages/{created.Id}");
            getAfter.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private record LanguageDto(Guid Id, string Code, string Name, bool IsEnabled);
        private record UpdateStatusRequest(bool IsEnabled);

    }
}
