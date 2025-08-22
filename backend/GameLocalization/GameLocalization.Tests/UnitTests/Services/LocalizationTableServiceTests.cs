using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.DTO.Common;
using GameLocalization.Core.DTO.Table;
using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Core.Services;
using GameLocalization.Core.Validation.LocalizationTable;
using GameLocalization.Tests.TestSupport.Base;
using Moq;
using FluentAssertions;

namespace GameLocalization.Tests.UnitTests.Services
{
    [Category("Unit")]
    public class LocalizationTableServiceTests : UnitTestBase
    {
        private Mock<ILanguageRepository> _languageRepositoryMock = null!;
        private Mock<ILocalizationKeyRepository> _keyRepositoryMock = null!;
        private LocalizationTableService _localizationTableService = null!;

        [SetUp]
        public void SetUp()
        {
            _languageRepositoryMock = new Mock<ILanguageRepository>(MockBehavior.Strict);
            _keyRepositoryMock = new Mock<ILocalizationKeyRepository>(MockBehavior.Strict);

            var validator = new LocalizationTableQueryDtoValidator();

            _localizationTableService = new LocalizationTableService(
                _languageRepositoryMock.Object,
                _keyRepositoryMock.Object,
                validator);
        }

        [Test]
        public async Task GetAsync_Should_Fail_When_Query_Invalid()
        {
            var query = new LocalizationTableQueryDto
            {
                Page = 0,
                PageSize = 10,
                IncludeDisabledLanguages = false,
                Search = null
            };

            var result = await _localizationTableService.GetAsync(query, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
        }

        [Test]
        public async Task GetAsync_Should_Return_Page_With_LanguageCodes_And_Rows()
        {
            var query = new LocalizationTableQueryDto
            {
                Page = 1,
                PageSize = 10,
                IncludeDisabledLanguages = false,
                Search = "menu"
            };

            var langs = new[]
            {
                new Language { Id = Guid.NewGuid(), Code = "en-us", IsEnabled = true },
                new Language { Id = Guid.NewGuid(), Code = "pl",    IsEnabled = true }
            };

            var keys = new List<LocalizationKey>
            {
                new() {
                    Id = Guid.NewGuid(),
                    Key = "menu.play",
                    Translations = new List<Translation>
                    {
                        new() { Language = langs[0], LanguageId = langs[0].Id, Value = "Play" },
                        new() { Language = langs[1], LanguageId = langs[1].Id, Value = "Graj" }
                    }
                },
                new() {
                    Id = Guid.NewGuid(),
                    Key = "menu.exit",
                    Translations = new List<Translation>
                    {
                        new() { Language = langs[0], LanguageId = langs[0].Id, Value = "Exit" },
                    }
                }
            };

            _languageRepositoryMock
                .Setup(r => r.GetAllAsync(false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(langs);

            _keyRepositoryMock
                .Setup(r => r.GetPageWithTranslationsAsync(
                    query.Page, query.PageSize, "menu",false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedSlice<LocalizationKey> { Items = keys, Total = keys.Count });

            var result = await _localizationTableService.GetAsync(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.LanguageCodes.Should().BeEquivalentTo(new[] { "en-us", "pl" });
            result.Value.Rows.Items.Should().HaveCount(2);

            var row1 = result.Value.Rows.Items.First(x => x.Key == "menu.play");
            row1.ValuesByLanguage["en-us"].Should().Be("Play");
            row1.ValuesByLanguage["pl"].Should().Be("Graj");

            var row2 = result.Value.Rows.Items.First(x => x.Key == "menu.exit");
            row2.ValuesByLanguage["en-us"].Should().Be("Exit");
            row2.ValuesByLanguage["pl"].Should().BeEmpty();

            _languageRepositoryMock.Verify(r => r.GetAllAsync(false, It.IsAny<CancellationToken>()), Times.Once);
            _keyRepositoryMock.Verify(r => 
                r.GetPageWithTranslationsAsync(query.Page, query.PageSize, "menu", false, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
