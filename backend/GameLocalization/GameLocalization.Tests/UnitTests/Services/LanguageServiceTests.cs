using FluentAssertions;
using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.DTO.Languages;
using GameLocalization.Core.Errors;
using GameLocalization.Core.Exceptions;
using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Core.Services;
using GameLocalization.Core.Validation.Languages;
using Microsoft.Extensions.Logging;
using Moq;
using GameLocalization.Tests.TestSupport.Base;

namespace GameLocalization.Tests.UnitTests.Services
{
    [Category("Unit")]
    public class LanguageServiceTests : UnitTestBase
    {
        private Mock<ILanguageRepository> _languageRepositoryMock = null!;
        private Mock<IUnitOfWork> _unitOfWorkMock = null!;
        private Mock<ILogger<LanguageService>> _loggerMock = null!;
        private LanguageService _languageService = null!;

        [SetUp]
        public void SetUp()
        {
            _languageRepositoryMock = new Mock<ILanguageRepository>(MockBehavior.Strict);
            _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            _loggerMock = new Mock<ILogger<LanguageService>>(MockBehavior.Loose);

            var createValidator = new CreateLanguageValidator();
            var updateValidator = new UpdateLanguageValidator();

            _languageService = new LanguageService(
                _languageRepositoryMock.Object,
                _unitOfWorkMock.Object,
                Mapper,
                createValidator,
                updateValidator,
                _loggerMock.Object);
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_NotFound_When_NotExists()
        {
            var id = Guid.NewGuid();
            _languageRepositoryMock
                .Setup(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Language?)null);

            var result = await _languageService.GetByIdAsync(id, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.OfType<AppError>().Should().ContainSingle(e => e.HttpStatus == 404);

            _languageRepositoryMock.Verify(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_Dto_When_Found()
        {
            var id = Guid.NewGuid();
            var entity = new Language { Id = id, Code = "en-us", Name = "English", IsEnabled = true };

            _languageRepositoryMock
                .Setup(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var result = await _languageService.GetByIdAsync(id, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(id);
            result.Value.Code.Should().Be("en-us");
            result.Value.Name.Should().Be("English");

            _languageRepositoryMock.Verify(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAllAsync_Should_Return_Only_Enabled_When_IncludeDisabled_False()
        {
            var list = new List<Language>
            {
                new() { Id = Guid.NewGuid(), Code = "en-us", Name = "English", IsEnabled = true },
                new() { Id = Guid.NewGuid(), Code = "pl",    Name = "Polski",  IsEnabled = false }
            };

            _languageRepositoryMock
                .Setup(r => r.GetAllAsync(false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(list.Where(x => x.IsEnabled).ToList());

            var result = await _languageService.GetAllAsync(includeDisabled: false, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().ContainSingle(x => x.Code == "en-us");

            _languageRepositoryMock.Verify(r => r.GetAllAsync(false, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAllAsync_Should_Return_All_When_IncludeDisabled_True()
        {
            var list = new List<Language>
            {
                new() { Id = Guid.NewGuid(), Code = "en-us", Name = "English", IsEnabled = true },
                new() { Id = Guid.NewGuid(), Code = "pl",    Name = "Polski",  IsEnabled = false }
            };

            _languageRepositoryMock
                .Setup(r => r.GetAllAsync(true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(list);

            var result = await _languageService.GetAllAsync(includeDisabled: true, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);

            _languageRepositoryMock.Verify(r => r.GetAllAsync(true, It.IsAny<CancellationToken>()), Times.Once);
        }


        [TestCase("", "English")]
        [TestCase("en", "")]
        public async Task CreateAsync_Should_Fail_When_Invalid(string code, string name)
        {
            var dto = new CreateLanguageDto { Code = code, Name = name };

            var result = await _languageService.CreateAsync(dto, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.OfType<ValidationAppError>().Should().HaveCount(1);

            _languageRepositoryMock.Verify(r => r.ExistsByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _languageRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Language>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestCase("en-US", "English", "en-us")]
        [TestCase("PL", "Polski", "pl")]
        public async Task CreateAsync_Should_Return_Conflict_When_Code_AlreadyExists(string code, string name, string normalized)
        {
            var dto = new CreateLanguageDto { Code = code, Name = name };

            _languageRepositoryMock
                .Setup(r => r.ExistsByCodeAsync(normalized, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _languageService.CreateAsync(dto, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.OfType<ConflictError>().Should().HaveCount(1);

            _languageRepositoryMock.Verify(r => r.ExistsByCodeAsync(normalized, It.IsAny<CancellationToken>()), Times.Once);
            _languageRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Language>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task CreateAsync_Should_Succeed_When_Valid()
        {
            var dto = new CreateLanguageDto { Code = "En-US", Name = "English" };

            _languageRepositoryMock
                .Setup(r => r.ExistsByCodeAsync("en-us", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _languageRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Language>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _languageService.CreateAsync(dto, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Code.Should().Be("en-us");
            result.Value.Name.Should().Be("English");

            _languageRepositoryMock.Verify(r => r.ExistsByCodeAsync("en-us", It.IsAny<CancellationToken>()), Times.Once);
            _languageRepositoryMock.Verify(r => r.AddAsync(It.Is<Language>(l => l.Code == "en-us" && l.Name == "English"), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task CreateAsync_Should_Return_DatabaseError_On_Save_Exception()
        {
            var dto = new CreateLanguageDto { Code = "en", Name = "Test" };

            _languageRepositoryMock
                .Setup(r => r.ExistsByCodeAsync("en", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _languageRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Language>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DataAccessException("boom"));

            var result = await _languageService.CreateAsync(dto, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.OfType<DatabaseError>().Should().HaveCount(1);

            _languageRepositoryMock.Verify(r => r.ExistsByCodeAsync("en", It.IsAny<CancellationToken>()), Times.Once);
            _languageRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Language>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpdateStatusAsync_Should_Return_NotFound_When_Missing()
        {
            var id = Guid.NewGuid();

            _languageRepositoryMock
                .Setup(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Language?)null);

            var result = await _languageService.UpdateStatusAsync(id, isEnabled: true, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.OfType<AppError>().Should().ContainSingle(e => e.HttpStatus == 404);

            _languageRepositoryMock.Verify(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task UpdateStatusAsync_Should_Update_And_Return_Dto_When_Success()
        {
            var id = Guid.NewGuid();
            var entity = new Language { Id = id, Code = "pl", Name = "Polski", IsEnabled = false };

            _languageRepositoryMock
                .Setup(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _languageService.UpdateStatusAsync(id, isEnabled: true, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.IsEnabled.Should().BeTrue();
            entity.IsEnabled.Should().BeTrue();

            _languageRepositoryMock.Verify(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpdateStatusAsync_Should_Return_DatabaseError_On_Save_Exception()
        {
            var id = Guid.NewGuid();
            var entity = new Language { Id = id, Code = "en", Name = "English", IsEnabled = false };

            _languageRepositoryMock
                .Setup(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DataAccessException("boom"));

            var result = await _languageService.UpdateStatusAsync(id, isEnabled: true, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.OfType<DatabaseError>().Should().HaveCount(1);

            _languageRepositoryMock.Verify(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_Should_Return_NotFound_When_Missing()
        {
            var id = Guid.NewGuid();

            _languageRepositoryMock
                .Setup(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Language?)null);

            var result = await _languageService.DeleteAsync(id, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.OfType<AppError>().Should().ContainSingle(e => e.HttpStatus == 404);

            _languageRepositoryMock.Verify(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _languageRepositoryMock.Verify(r => r.Remove(It.IsAny<Language>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task DeleteAsync_Should_Remove_And_Save_When_Success()
        {
            var id = Guid.NewGuid();
            var entity = new Language { Id = id, Code = "en", Name = "English", IsEnabled = true };

            _languageRepositoryMock
                .Setup(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            _languageRepositoryMock
                .Setup(r => r.Remove(entity));

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _languageService.DeleteAsync(id, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();

            _languageRepositoryMock.Verify(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _languageRepositoryMock.Verify(r => r.Remove(entity), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_Should_Return_DatabaseError_On_Save_Exception()
        {
            var id = Guid.NewGuid();
            var entity = new Language { Id = id, Code = "en", Name = "English", IsEnabled = true };

            _languageRepositoryMock
                .Setup(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            _languageRepositoryMock
                .Setup(r => r.Remove(entity));

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DataAccessException("boom"));

            var result = await _languageService.DeleteAsync(id, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.OfType<DatabaseError>().Should().HaveCount(1);

            _languageRepositoryMock.Verify(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _languageRepositoryMock.Verify(r => r.Remove(entity), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
