using GameLocalization.Tests.TestSupport.Base;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GameLocalization.Core.Domain.Entities;
using GameLocalization.Tests.TestSupport.Auth;

namespace GameLocalization.Tests.IntegrationTests.Controllers
{
    [Category("Integration")]
    public class LocalizationTableControllerTests : ApiIntegrationTestBase
    {
        [Test]
        public async Task Get_Table_Should_Return_LanguageCodes_And_Paged_Rows()
        {
            Guid enId = Guid.NewGuid();
            Guid plId = Guid.NewGuid();

            await SeedAsync(db =>
            {
                var en = new Language { Id = enId, Code = "en-us", Name = "English", IsEnabled = true };
                var pl = new Language { Id = plId, Code = "pl", Name = "Polski", IsEnabled = false };
                db.Languages.AddRange(en, pl);

                db.LocalizationKeys.AddRange(
                    new LocalizationKey
                    {
                        Id = Guid.NewGuid(),
                        Key = "menu.play",
                        Translations = new List<Translation>
                        {
                            new() { Id = Guid.NewGuid(), LanguageId = enId, Value = "Play" },
                            new() { Id = Guid.NewGuid(), LanguageId = plId, Value = "Graj" }
                        }
                    },
                    new LocalizationKey
                    {
                        Id = Guid.NewGuid(),
                        Key = "menu.exit",
                        Translations = new List<Translation>
                        {
                            new() { Id = Guid.NewGuid(), LanguageId = enId, Value = "Exit" }
                        }
                    }
                );
                return Task.CompletedTask;
            });

            Client.AsMember();

            var resp = await Client
                .GetAsync("api/v1/localization-table?page=1&pageSize=10&search=menu&includeDisabled=false");
            resp.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await resp.Content.ReadFromJsonAsync<TableResponse>();
            dto.Should().NotBeNull();

            dto!.LanguageCodes.Should().BeEquivalentTo("en-us");
            dto.Rows.Total.Should().Be(2);
            dto.Rows.Items.Should().HaveCount(2);

            var play = dto.Rows.Items.First(x => x.Key == "menu.play");
            play.ValuesByLanguage.Should().ContainKey("en-us").WhoseValue.Should().Be("Play");
            play.ValuesByLanguage.Should().NotContainKey("pl");

            var exit = dto.Rows.Items.First(x => x.Key == "menu.exit");
            exit.ValuesByLanguage["en-us"].Should().Be("Exit");
        }

        private record TableResponse(string[] LanguageCodes, PagedRows Rows);
        private record PagedRows(int Total, List<TableRowDto> Items);
        private record TableRowDto(Guid KeyId, string Key, Dictionary<string, string> ValuesByLanguage);
    }
}
