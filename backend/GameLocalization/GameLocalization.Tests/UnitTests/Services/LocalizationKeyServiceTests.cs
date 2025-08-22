using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.DTO.Keys;
using GameLocalization.Core.Errors;
using GameLocalization.Core.Exceptions;
using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Core.Services;
using GameLocalization.Core.Validation.LocalizationKeys;
using GameLocalization.Tests.TestSupport.Base;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace GameLocalization.Tests.UnitTests.Services
{
    [Category("Unit")]
    public class LocalizationKeyServiceTests : UnitTestBase
    {
        private Mock<ILocalizationKeyRepository> _keyRepositoryMock = null!;
        private Mock<ILanguageRepository> _languageRepositoryMock = null!;
        private Mock<IUnitOfWork> _unitOfWorkMock = null!;
        private Mock<ILogger<LocalizationKeyService>> _loggerMock = null!;
        private LocalizationKeyService _keyService = null!;

        [SetUp]
        public void SetUp()
        {
            _keyRepositoryMock = new Mock<ILocalizationKeyRepository>(MockBehavior.Strict);
            _languageRepositoryMock = new Mock<ILanguageRepository>(MockBehavior.Strict);
            _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            _loggerMock = new Mock<ILogger<LocalizationKeyService>>(MockBehavior.Loose);

            var createValidator = new CreateLocalizationKeyValidator();

            _keyService = new LocalizationKeyService(
                _keyRepositoryMock.Object,
                _languageRepositoryMock.Object,
                _unitOfWorkMock.Object,
                createValidator,
                _loggerMock.Object);
        }

        [TestCase("", TestName = "CreateAsync_Should_Fail_When_Key_Empty")]
        [TestCase("   ", TestName = "CreateAsync_Should_Fail_When_Key_Whitespace")]
        public async Task CreateAsync_Should_Fail_When_Invalid(string rawKey)
        {
            var dto = new CreateLocalizationKeyDto { Key = rawKey };

            var result = await _keyService.CreateAsync(dto, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.OfType<ValidationAppError>().Should().HaveCount(1);

            _keyRepositoryMock.Verify(r => r.ExistsByKeyAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _keyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<LocalizationKey>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task CreateAsync_Should_Return_Conflict_When_Key_Exists()
        {
            var dto = new CreateLocalizationKeyDto { Key = "Menu.Play"};

            _keyRepositoryMock
                .Setup(r => r.ExistsByKeyAsync("menu.play", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _keyService.CreateAsync(dto, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.OfType<ConflictError>().Should().HaveCount(1);

            _keyRepositoryMock.Verify(r => r.ExistsByKeyAsync("menu.play", It.IsAny<CancellationToken>()), Times.Once);
            _keyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<LocalizationKey>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task CreateAsync_Should_Succeed_And_Save()
        {
            var dto = new CreateLocalizationKeyDto { Key = "Menu.Play" };

            _keyRepositoryMock
                .Setup(r => r.ExistsByKeyAsync("menu.play", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _keyRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<LocalizationKey>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _languageRepositoryMock
                .Setup(r => r.GetAllAsync(true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<Language>());

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _keyService.CreateAsync(dto, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Key.Should().Be("menu.play");

            _keyRepositoryMock.Verify(r => r.ExistsByKeyAsync("menu.play", It.IsAny<CancellationToken>()), Times.Once);
            _keyRepositoryMock.Verify(r =>
                r.AddAsync(It.Is<LocalizationKey>(k => k.Key == "menu.play"), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task CreateAsync_Should_Return_DatabaseError_On_Save_Exception()
        {
            var dto = new CreateLocalizationKeyDto { Key = "menu.exit" };

            _keyRepositoryMock
                .Setup(r => r.ExistsByKeyAsync("menu.exit", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _keyRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<LocalizationKey>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _languageRepositoryMock
                .Setup(r => r.GetAllAsync(true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<Language>());

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DataAccessException("boom"));

            var result = await _keyService.CreateAsync(dto, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.OfType<DatabaseError>().Should().HaveCount(1);

            _keyRepositoryMock.Verify(r => r.ExistsByKeyAsync("menu.exit", It.IsAny<CancellationToken>()), Times.Once);
            _keyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<LocalizationKey>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_Should_Return_NotFound_When_Missing()
        {
            var id = Guid.NewGuid();

            _keyRepositoryMock
                .Setup(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((LocalizationKey?)null);

            var result = await _keyService.DeleteAsync(id, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.OfType<AppError>().Should().ContainSingle(e => e.HttpStatus == 404);

            _keyRepositoryMock.Verify(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _keyRepositoryMock.Verify(r => r.Remove(It.IsAny<LocalizationKey>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task DeleteAsync_Should_Remove_And_Save_When_Found()
        {
            var id = Guid.NewGuid();
            var entity = new LocalizationKey { Id = id, Key = "menu.play"};

            _keyRepositoryMock
                .Setup(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            _keyRepositoryMock
                .Setup(r => r.Remove(entity));

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _keyService.DeleteAsync(id, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();

            _keyRepositoryMock.Verify(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _keyRepositoryMock.Verify(r => r.Remove(entity), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
